#region License

/*
MIT License

Copyright(c) 2017-2020 Mattias Edlund

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

#endregion

using UnityEngine;
using UnityEngine.Rendering;

namespace Procedural {
    /// <summary>
    ///     A LOD (level of detail) generator helper.
    /// </summary>
    [AddComponentMenu("Rendering/LOD Generator Helper")]
	public sealed class LODGeneratorHelper : MonoBehaviour {
#region Unity Events

		void Reset() {
			fadeMode              = LODFadeMode.None;
			animateCrossFading    = false;
			autoCollectRenderers  = true;
			simplificationOptions = SimplificationOptions.Default;

			levels = new[] {
				new(0.5f, 1f) {
					CombineMeshes        = false,
					CombineSubMeshes     = false,
					SkinQuality          = SkinQuality.Auto,
					ShadowCastingMode    = ShadowCastingMode.On,
					ReceiveShadows       = true,
					SkinnedMotionVectors = true,
					LightProbeUsage      = LightProbeUsage.BlendProbes,
					ReflectionProbeUsage = ReflectionProbeUsage.BlendProbes
				},
				new LODLevel(0.17f, 0.65f) {
					CombineMeshes        = true,
					CombineSubMeshes     = false,
					SkinQuality          = SkinQuality.Auto,
					ShadowCastingMode    = ShadowCastingMode.On,
					ReceiveShadows       = true,
					SkinnedMotionVectors = true,
					LightProbeUsage      = LightProbeUsage.BlendProbes,
					ReflectionProbeUsage = ReflectionProbeUsage.Simple
				},
				new LODLevel(0.02f, 0.4225f) {
					CombineMeshes        = true,
					CombineSubMeshes     = true,
					SkinQuality          = SkinQuality.Bone2,
					ShadowCastingMode    = ShadowCastingMode.Off,
					ReceiveShadows       = false,
					SkinnedMotionVectors = false,
					LightProbeUsage      = LightProbeUsage.Off,
					ReflectionProbeUsage = ReflectionProbeUsage.Off
				}
			};
		}

#endregion

#region Fields

		[SerializeField] [Tooltip("The fade mode used by the created LOD group.")]
		LODFadeMode fadeMode = LODFadeMode.None;

		[SerializeField] [Tooltip("If the cross-fading should be animated by time.")]
		bool animateCrossFading;

		[SerializeField]
		[Tooltip("If the renderers under this game object and any children should be automatically collected.")]
		bool autoCollectRenderers = true;

		[SerializeField] [Tooltip("The simplification options.")]
		SimplificationOptions simplificationOptions = SimplificationOptions.Default;

		[SerializeField]
		[Tooltip(
			"The path within the assets directory to save the generated assets. Leave this empty to use the default path.")]
		string saveAssetsPath = string.Empty;

		[SerializeField] [Tooltip("The LOD levels.")]
		LODLevel[] levels;

		[SerializeField] bool isGenerated;

#endregion

#region Properties

        /// <summary>
        ///     Gets or sets the fade mode used by the created LOD group.
        /// </summary>
        public LODFadeMode FadeMode {
			get => fadeMode;
			set => fadeMode = value;
		}

        /// <summary>
        ///     Gets or sets if the cross-fading should be animated by time. The animation duration
        ///     is specified globally as crossFadeAnimationDuration.
        /// </summary>
        public bool AnimateCrossFading {
			get => animateCrossFading;
			set => animateCrossFading = value;
		}

        /// <summary>
        ///     Gets or sets if the renderers under this game object and any children should be automatically collected.
        /// </summary>
        public bool AutoCollectRenderers {
			get => autoCollectRenderers;
			set => autoCollectRenderers = value;
		}

        /// <summary>
        ///     Gets or sets the simplification options.
        /// </summary>
        public SimplificationOptions SimplificationOptions {
			get => simplificationOptions;
			set => simplificationOptions = value;
		}

        /// <summary>
        ///     Gets or sets the path within the project to save the generated assets.
        ///     Leave this empty to use the default path.
        /// </summary>
        public string SaveAssetsPath {
			get => saveAssetsPath;
			set => saveAssetsPath = value;
		}

        /// <summary>
        ///     Gets or sets the LOD levels for this generator.
        /// </summary>
        public LODLevel[] Levels {
			get => levels;
			set => levels = value;
		}

        /// <summary>
        ///     Gets if the LODs have been generated.
        /// </summary>
        public bool IsGenerated => isGenerated;

#endregion
	}
}