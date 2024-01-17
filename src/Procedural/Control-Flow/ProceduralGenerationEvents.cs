using System;
using RIO.BCL;
using Sirenix.OdinInspector;
using UnityBCL;
using UnityEngine;
using UnityEngine.Events;

namespace Procedural {
	public class ProceduralGenerationEvents : Singleton<ProceduralGenerationEvents, ProceduralGenerationEvents> {
		readonly Observable _onCompleteObservable = new();

		[field: SerializeField]
		[field: BoxGroup("0", false)]
		public Events Hook { get; private set; }

		public IObservable OnCompleteObservable => _onCompleteObservable;

		[Serializable]
		[HideLabel]
		public class Events {
			[field: SerializeField]
			[field: BoxGroup("0", false)]
			public UnityEvent OnStartComplete { get; private set; }

			[field: SerializeField]
			[field: BoxGroup("0", false)]
			public UnityEvent OnPopulatingMap { get; private set; }

			[field: SerializeField]
			[field: BoxGroup("0", false)]
			public UnityEvent OnSmoothingMap { get; private set; }

			[field: SerializeField]
			[field: BoxGroup("0", false)]
			public UnityEvent OnRemovingRegions { get; private set; }

			[field: SerializeField]
			[field: BoxGroup("0", false)]
			public UnityEvent OnCreatingBoundary { get; private set; }

			[field: SerializeField]
			[field: BoxGroup("0", false)]
			public MapCharacteristicsUnityEvent MapCharacteristicsCalculated { get; private set; }

			[field: SerializeField]
			[field: BoxGroup("0", false)]
			public UnityEvent OnScalingGrid { get; private set; }

			[field: SerializeField]
			[field: BoxGroup("0", false)]
			public UnityEvent OnPreparingAndSettingTiles { get; private set; }

			[field: SerializeField]
			[field: BoxGroup("0", false)]
			public UnityEvent OnGeneratingMesh { get; private set; }

			[field: SerializeField]
			[field: BoxGroup("0", false)]
			public UnityEvent OnCalculatingPathfinding { get; private set; }

			[field: SerializeField]
			[field: BoxGroup("0", false)]
			public UnityEvent OnCompilingData { get; private set; }

			[field: SerializeField]
			[field: BoxGroup("0", false)]
			public UnityEvent OnDisposing { get; private set; }

			[field: SerializeField]
			[field: BoxGroup("0", false)]
			public UnityEvent OnComplete { get; private set; }

			[field: SerializeField]
			[field: BoxGroup("0", false)]
			public OnCompleteWithDataEvent OnCompleteWithData { get; private set; }
		}
		/*
		 * Pending,
		Starting,
		PopulatingMap,
		SmoothingMap,
		RemovingRegions,
		CreatingBoundary,
		ScalingGrid,
		PreparingAndSettingTiles,
		GeneratingMesh,			// mesh collider is generated during this state1
		CalculatingPathfinding,
		CompilingData,
		Disposing,
		Complete
		 */
	}
}