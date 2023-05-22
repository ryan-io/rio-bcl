﻿using System;

namespace BCL.Unmanaged {
	public class Authorizer {
		public bool IsAuthorized<T>(T flag, T authorizedFlags) where T : unmanaged, Enum {
			return ShouldProcess && authorizedFlags.HasFlags(flag);
		}
		
		public bool IsNotAuthorized<T>(T flag, T unauthorizedFlags) where T : unmanaged, Enum {
			return !ShouldProcess || unauthorizedFlags.HasFlags(flag);
		}

		public void SetAuthorization(bool state) => ShouldProcess = state;

		public Authorizer(bool initiallyAuthorized) {
			ShouldProcess = initiallyAuthorized;
		}

		public bool ShouldProcess { get; private set; }
	}
}