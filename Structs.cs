using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace HolyHook
	{
	public class Structs
		{
		[StructLayout(LayoutKind.Sequential)]
		public unsafe struct PlayerState_t
			{
			public int command_time;
			public int pm_type;
			public int bob_cycle;
			public int pm_flags;
			public int pm_time;

			public Vector3 origin;
			public Vector3 velocity;

			public int weapon_time;
			public int gravity;
			public int leanf;
			public int speed;

			public int delta_angles_0;
			public int delta_angles_1;
			public int delta_angles_2;

			public int ground_entity_num;
			public fixed int stats[16];
			
			}

		[StructLayout(LayoutKind.Sequential)]
		public unsafe struct CG_t
			{
			public int client_frame;
			public int client_num;
			public int local_server_scene_time;
			public int last_executed_server_command;
			public int server_command_sequence;
			public int latest_snapshot_num;
			public int latest_snapshot_time;

			
			public PlayerState_t snap;

			//localplayer predicted by the engine
			public PlayerState_t predicted_player_state;
			}

		//when this game was made it was optimized to save bandwith due to poopy internet connection speeds
		//usercmd is a network packet, because of its amount they decided to force the compiler not to pad any bytes into it
		//__attribute__((packed)) made the cplr compress it and we need to account for it and pack it so we dont write data into wrong bytes

		//if the cpu has to read a non /4/8 value it takes >1 cycle -> the local memory structs sacrifice ram to ensure cpu speed
		//network or fileheader = pack
		//others are alligned for cpu speed and we dont pack
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct usercmd_t
			{
			public int server_time;
			public int angles_0;//pitch
			public int angles_1;//yaw
			public int angles_2;//roll (can be nospreaded)
			public int buttons;
			public byte weapon;
			public sbyte forwardmove;
			public sbyte rightmove;
			public sbyte upmove;
			}
		}
	}
