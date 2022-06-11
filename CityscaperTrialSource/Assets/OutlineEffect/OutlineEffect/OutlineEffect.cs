/*
//  Copyright (c) 2015 José Guerreiro. All rights reserved.
//
//  MIT license, see http://www.opensource.org/licenses/mit-license.php
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
*/

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace cakeslice {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    /* [ExecuteInEditMode] */
    public class OutlineEffect : MonoBehaviour {

        [Range(1.0f, 6.0f)]
        public float lineThickness = 1.25f;

        [Range(0, 10)]
        public float lineIntensity = .5f;

        [Range(0, 1)]
        public float fillAmount = 0.2f;

        public Color lineColor0 = Color.red;
        public Color lineColor1 = Color.green;
        public Color lineColor2 = Color.blue;

        public bool additiveRendering;

        public bool backfaceCulling = true;

        public Color fillColor = Color.blue;
        public bool useFillColor;

        [Header("These settings can affect performance!")]
        public bool cornerOutlines;

        public bool addLinesBetweenColors;

        [Header("Advanced settings")]
        public bool scaleWithScreenSize = true;

        [Range(0.0f, 1.0f)]
        public float alphaCutoff = .5f;

        public bool flipY;
        public Camera sourceCamera;
        public bool autoEnableOutlines;

        [HideInInspector]
        public Camera outlineCamera;

        [HideInInspector]
        public Material outlineShaderMaterial;

        [HideInInspector]
        public RenderTexture renderTexture;

        [HideInInspector]
        public RenderTexture extraRenderTexture;

        private readonly LinkedSet<Outline> outlines = new LinkedSet<Outline>();

        private CommandBuffer commandBuffer;
        private readonly List<Material> materialBuffer = new List<Material>();
        private Material outline1Material;
        private Material outline2Material;
        private Material outline3Material;
        private Shader outlineBufferShader;
        private Material outlineEraseMaterial;
        private Shader outlineShader;

        private bool RenderTheNextFrame;
        /* #if UNITY_EDITOR
                  private void OnValidate()
                  {
                        CreateMaterialsIfNeeded();
                  }
        #endif */

        public static OutlineEffect Instance { get; private set; }

        private void Awake() {
            if (Instance != null) {
                Destroy(this);

                throw new Exception("you can only have one outline camera in the scene");
            }

            Instance = this;
        }

        private void Start() {
            CreateMaterialsIfNeeded();
            UpdateMaterialsPublicProperties();

            if (this.sourceCamera == null) {
                this.sourceCamera = GetComponent<Camera>();

                if (this.sourceCamera == null) {
                    this.sourceCamera = Camera.main;
                }
            }

            if (this.outlineCamera == null) {
                foreach (Camera c in GetComponentsInChildren<Camera>()) {
                    if (c.name == "Outline Camera") {
                        this.outlineCamera = c;
                        c.enabled = false;

                        break;
                    }
                }

                if (this.outlineCamera == null) {
                    GameObject cameraGameObject = new GameObject("Outline Camera");
                    cameraGameObject.transform.parent = this.sourceCamera.transform;
                    this.outlineCamera = cameraGameObject.AddComponent<Camera>();
                    this.outlineCamera.enabled = false;
                }
            }

            if (this.renderTexture != null) {
                this.renderTexture.Release();
            }

            if (this.extraRenderTexture != null) {
                this.renderTexture.Release();
            }

            this.renderTexture = new RenderTexture(this.sourceCamera.pixelWidth, this.sourceCamera.pixelHeight, 16,
                RenderTextureFormat.Default);
            this.extraRenderTexture = new RenderTexture(this.sourceCamera.pixelWidth, this.sourceCamera.pixelHeight, 16,
                RenderTextureFormat.Default);
            UpdateOutlineCameraFromSource();

            this.commandBuffer = new CommandBuffer();
            this.outlineCamera.AddCommandBuffer(CameraEvent.BeforeImageEffects, this.commandBuffer);
        }

        private void OnEnable() {
            Outline[] o = FindObjectsOfType<Outline>();
            if (this.autoEnableOutlines) {
                foreach (Outline oL in o) {
                    oL.enabled = false;
                    oL.enabled = true;
                }
            } else {
                foreach (Outline oL in o) {
                    if (!this.outlines.Contains(oL)) {
                        this.outlines.Add(oL);
                    }
                }
            }
        }

        private void OnDestroy() {
            if (this.renderTexture != null) {
                this.renderTexture.Release();
            }

            if (this.extraRenderTexture != null) {
                this.extraRenderTexture.Release();
            }

            DestroyMaterials();
        }

        public void OnPreRender() {
            if (this.commandBuffer == null) {
                return;
            }

            // The first frame during which there are no outlines, we still need to render 
            // to clear out any outlines that were being rendered on the previous frame
            if (this.outlines.Count == 0) {
                if (!this.RenderTheNextFrame) {
                    return;
                }

                this.RenderTheNextFrame = false;
            } else {
                this.RenderTheNextFrame = true;
            }

            CreateMaterialsIfNeeded();

            if (this.renderTexture == null || this.renderTexture.width != this.sourceCamera.pixelWidth ||
                this.renderTexture.height != this.sourceCamera.pixelHeight) {
                if (this.renderTexture != null) {
                    this.renderTexture.Release();
                }

                if (this.extraRenderTexture != null) {
                    this.renderTexture.Release();
                }

                this.renderTexture = new RenderTexture(this.sourceCamera.pixelWidth, this.sourceCamera.pixelHeight, 16,
                    RenderTextureFormat.Default);
                this.extraRenderTexture = new RenderTexture(this.sourceCamera.pixelWidth, this.sourceCamera.pixelHeight,
                    16, RenderTextureFormat.Default);
                this.outlineCamera.targetTexture = this.renderTexture;
            }

            UpdateMaterialsPublicProperties();
            UpdateOutlineCameraFromSource();
            this.outlineCamera.targetTexture = this.renderTexture;
            this.commandBuffer.SetRenderTarget(this.renderTexture);

            this.commandBuffer.Clear();

            foreach (Outline outline in this.outlines) {
                LayerMask l = this.sourceCamera.cullingMask;

                if (outline != null && l == (l | (1 << outline.gameObject.layer))) {
                    for (int v = 0; v < outline.SharedMaterials.Length; v++) {
                        Material m = null;

                        if (outline.SharedMaterials[v].HasProperty("_MainTex") &&
                            outline.SharedMaterials[v].mainTexture != null && outline.SharedMaterials[v]) {
                            foreach (Material g in this.materialBuffer) {
                                if (g.mainTexture == outline.SharedMaterials[v].mainTexture) {
                                    if (outline.eraseRenderer && g.color == this.outlineEraseMaterial.color) {
                                        m = g;
                                    } else if (!outline.eraseRenderer &&
                                               g.color == GetMaterialFromID(outline.color).color) {
                                        m = g;
                                    }
                                }
                            }

                            if (m == null) {
                                if (outline.eraseRenderer) {
                                    m = new Material(this.outlineEraseMaterial);
                                } else {
                                    m = new Material(GetMaterialFromID(outline.color));
                                }

                                m.mainTexture = outline.SharedMaterials[v].mainTexture;
                                this.materialBuffer.Add(m);
                            }
                        } else {
                            if (outline.eraseRenderer) {
                                m = this.outlineEraseMaterial;
                            } else {
                                m = GetMaterialFromID(outline.color);
                            }
                        }

                        if (this.backfaceCulling) {
                            m.SetInt("_Culling", (int)CullMode.Back);
                        } else {
                            m.SetInt("_Culling", (int)CullMode.Off);
                        }

                        MeshFilter mL = outline.MeshFilter;
                        SkinnedMeshRenderer sMR = outline.SkinnedMeshRenderer;
                        SpriteRenderer sR = outline.SpriteRenderer;
                        if (mL) {
                            if (mL.sharedMesh != null) {
                                if (v < mL.sharedMesh.subMeshCount) {
                                    this.commandBuffer.DrawRenderer(outline.Renderer, m, v, 0);
                                }
                            }
                        } else if (sMR) {
                            if (sMR.sharedMesh != null) {
                                if (v < sMR.sharedMesh.subMeshCount) {
                                    this.commandBuffer.DrawRenderer(outline.Renderer, m, v, 0);
                                }
                            }
                        } else if (sR) {
                            this.commandBuffer.DrawRenderer(outline.Renderer, m, v, 0);
                        }
                    }
                }
            }

            this.outlineCamera.Render();
        }

        [ImageEffectOpaque]
        private void OnRenderImage(RenderTexture source, RenderTexture destination) {
            if (this.outlineShaderMaterial != null) {
                this.outlineShaderMaterial.SetTexture("_OutlineSource", this.renderTexture);

                if (this.addLinesBetweenColors) {
                    Graphics.Blit(source, this.extraRenderTexture, this.outlineShaderMaterial, 0);
                    this.outlineShaderMaterial.SetTexture("_OutlineSource", this.extraRenderTexture);
                }

                Graphics.Blit(source, destination, this.outlineShaderMaterial, 1);
            }
        }

        private Material GetMaterialFromID(int ID) {
            if (ID == 0) {
                return this.outline1Material;
            }

            if (ID == 1) {
                return this.outline2Material;
            }

            if (ID == 2) {
                return this.outline3Material;
            }

            return this.outline1Material;
        }

        private Material CreateMaterial(Color emissionColor) {
            Material m = new Material(this.outlineBufferShader);
            m.SetColor("_Color", emissionColor);
            m.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            m.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            m.SetInt("_ZWrite", 0);
            m.DisableKeyword("_ALPHATEST_ON");
            m.EnableKeyword("_ALPHABLEND_ON");
            m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            m.renderQueue = 3000;

            return m;
        }

        private void CreateMaterialsIfNeeded() {
            if (this.outlineShader == null) {
                this.outlineShader = Resources.Load<Shader>("OutlineShader");
            }

            if (this.outlineBufferShader == null) {
                this.outlineBufferShader = Resources.Load<Shader>("OutlineBufferShader");
            }

            if (this.outlineShaderMaterial == null) {
                this.outlineShaderMaterial = new Material(this.outlineShader);
                this.outlineShaderMaterial.hideFlags = HideFlags.HideAndDontSave;
                UpdateMaterialsPublicProperties();
            }

            if (this.outlineEraseMaterial == null) {
                this.outlineEraseMaterial = CreateMaterial(new Color(0, 0, 0, 0));
            }

            if (this.outline1Material == null) {
                this.outline1Material = CreateMaterial(new Color(1, 0, 0, 0));
            }

            if (this.outline2Material == null) {
                this.outline2Material = CreateMaterial(new Color(0, 1, 0, 0));
            }

            if (this.outline3Material == null) {
                this.outline3Material = CreateMaterial(new Color(0, 0, 1, 0));
            }
        }

        private void DestroyMaterials() {
            foreach (Material m in this.materialBuffer) {
                DestroyImmediate(m);
            }

            this.materialBuffer.Clear();
            DestroyImmediate(this.outlineShaderMaterial);
            DestroyImmediate(this.outlineEraseMaterial);
            DestroyImmediate(this.outline1Material);
            DestroyImmediate(this.outline2Material);
            DestroyImmediate(this.outline3Material);
            this.outlineShader = null;
            this.outlineBufferShader = null;
            this.outlineShaderMaterial = null;
            this.outlineEraseMaterial = null;
            this.outline1Material = null;
            this.outline2Material = null;
            this.outline3Material = null;
        }

        public void UpdateMaterialsPublicProperties() {
            if (this.outlineShaderMaterial) {
                float scalingFactor = 1;
                if (this.scaleWithScreenSize) {
                    // If Screen.height gets bigger, outlines gets thicker
                    scalingFactor = Screen.height / 360.0f;
                }

                // If scaling is too small (height less than 360 pixels), make sure you still render the outlines, but render them with 1 thickness
                if (this.scaleWithScreenSize && scalingFactor < 1) {
                    if (XRSettings.isDeviceActive && this.sourceCamera.stereoTargetEye != StereoTargetEyeMask.None) {
                        this.outlineShaderMaterial.SetFloat("_LineThicknessX",
                            1 / 1000.0f * (1.0f / XRSettings.eyeTextureWidth) * 1000.0f);
                        this.outlineShaderMaterial.SetFloat("_LineThicknessY",
                            1 / 1000.0f * (1.0f / XRSettings.eyeTextureHeight) * 1000.0f);
                    } else {
                        this.outlineShaderMaterial.SetFloat("_LineThicknessX",
                            1 / 1000.0f * (1.0f / Screen.width) * 1000.0f);
                        this.outlineShaderMaterial.SetFloat("_LineThicknessY",
                            1 / 1000.0f * (1.0f / Screen.height) * 1000.0f);
                    }
                } else {
                    if (XRSettings.isDeviceActive && this.sourceCamera.stereoTargetEye != StereoTargetEyeMask.None) {
                        this.outlineShaderMaterial.SetFloat("_LineThicknessX",
                            scalingFactor * (this.lineThickness / 1000.0f) * (1.0f / XRSettings.eyeTextureWidth) *
                            1000.0f);
                        this.outlineShaderMaterial.SetFloat("_LineThicknessY",
                            scalingFactor * (this.lineThickness / 1000.0f) * (1.0f / XRSettings.eyeTextureHeight) *
                            1000.0f);
                    } else {
                        this.outlineShaderMaterial.SetFloat("_LineThicknessX",
                            scalingFactor * (this.lineThickness / 1000.0f) * (1.0f / Screen.width) * 1000.0f);
                        this.outlineShaderMaterial.SetFloat("_LineThicknessY",
                            scalingFactor * (this.lineThickness / 1000.0f) * (1.0f / Screen.height) * 1000.0f);
                    }
                }

                this.outlineShaderMaterial.SetFloat("_LineIntensity", this.lineIntensity);
                this.outlineShaderMaterial.SetFloat("_FillAmount", this.fillAmount);
                this.outlineShaderMaterial.SetColor("_FillColor", this.fillColor);
                this.outlineShaderMaterial.SetFloat("_UseFillColor", this.useFillColor ? 1 : 0);
                this.outlineShaderMaterial.SetColor("_LineColor1", this.lineColor0 * this.lineColor0);
                this.outlineShaderMaterial.SetColor("_LineColor2", this.lineColor1 * this.lineColor1);
                this.outlineShaderMaterial.SetColor("_LineColor3", this.lineColor2 * this.lineColor2);
                if (this.flipY) {
                    this.outlineShaderMaterial.SetInt("_FlipY", 1);
                } else {
                    this.outlineShaderMaterial.SetInt("_FlipY", 0);
                }

                if (!this.additiveRendering) {
                    this.outlineShaderMaterial.SetInt("_Dark", 1);
                } else {
                    this.outlineShaderMaterial.SetInt("_Dark", 0);
                }

                if (this.cornerOutlines) {
                    this.outlineShaderMaterial.SetInt("_CornerOutlines", 1);
                } else {
                    this.outlineShaderMaterial.SetInt("_CornerOutlines", 0);
                }

                Shader.SetGlobalFloat("_OutlineAlphaCutoff", this.alphaCutoff);
            }
        }

        private void UpdateOutlineCameraFromSource() {
            this.outlineCamera.CopyFrom(this.sourceCamera);
            this.outlineCamera.renderingPath = RenderingPath.Forward;
            this.outlineCamera.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
            this.outlineCamera.clearFlags = CameraClearFlags.SolidColor;
            this.outlineCamera.rect = new Rect(0, 0, 1, 1);
            this.outlineCamera.cullingMask = 0;
            this.outlineCamera.targetTexture = this.renderTexture;
            this.outlineCamera.enabled = false;
#if UNITY_5_6_OR_NEWER
            this.outlineCamera.allowHDR = false;
#else
			outlineCamera.hdr = false;
#endif
        }

        public void AddOutline(Outline outline) {
            this.outlines.Add(outline);
        }

        public void RemoveOutline(Outline outline) {
            this.outlines.Remove(outline);
        }
    }
}