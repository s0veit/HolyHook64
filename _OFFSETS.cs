using System;
using System.Collections.Generic;
using System.Text;

namespace HolyHook
	{
	public static class _OFFSETS
		{
			public static class _FUNCS
			{
				public const int _CL_CREATECMD = 0xF410;
				public const int _CL_CREATECMD_BYTES_2_STEAL = 12;
			}

			public static class _ENGINE_PTRS
			{
				public const int _BASE_PLAYER_STATE = 0x7B4000;
				public const int _GROUND_FLAG = 0x7B4044;
				public const int _HEALTH = 0x7B40B8;
			}

			public static class _PLAYER_STATE
			{
				public const int _ORIGIN = 0x18;
				public const int _VELOCITY = 0x24;
			}

			public static class _CONSTS
			{
				public const int _ENTITYNUM_NONE = 1023;//entitynum is what it is in the src -> air state
				public const sbyte _UPMOVE_MAX = 127;//the max value for upmove responsible for jumping
				public const sbyte _UPMOVE_MIN = -128;//duck
			}
		}
	}
