using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.AI;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        // R e a d m e
        // -----------
        // General Mining Drone Script v0.313A       
        // Adomus o7 o7 o7
        // 
        // 
        #region mdk preserve
        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }
        //rename these for drone
        int drone_id_num = 1;
        string dtag = "SWRM_D";
        //ore detection
        bool ORE_sense_enabled = true;
        float ORE_sense_limit = 0.0f;
        //dmg detect
        bool dmg_report_enabled = true;
        //collision sense ranges
        bool Collision_sense_enabled = true;
        float s_llm = 4.0f;
        float s_rlm = 4.0f;
        float s_btlm = 3.0f;
        float s_tlm = 4.5f;
        float s_bklm = 6.5f;
        float s_flm = 3.0f;
        //hydrogen recharge
        bool ignore_Htank = true;
        double gas_CHGhi = 100.0;
        double gas_CHGlow = 30.0;
        //battery recharge
        bool auto_charge_mode = true;
        float bat_CHGhi = 100.0f;
        float bat_CHGlow = 30.0f;
        //drone nav settings
        float drill_spd = 10.0f;
        float nav_spd = 5.0f;
        float exit_spd = 1.0f;
        double nav_inst_thr = 0.05;
        double gsl = 0.1;
        //drone mining settings
        double drill_sl = 100.0;
        double drill_el = 20.0;
        double req_dist = 1;
        double nav_prec = 0.6;
        double nav_prec2 = 1.2;
        double mine_prec = 0.6;
        //statics
        bool udock_conf = true;
        bool skip_prec_mode = true;
        string Dock = "Dock";
        string Undock = "UnDock";
        string TON = "TON";
        string TOFF = "TOFF";
        string Reset = "Reset";
        string CA = "CA";
        string PrecM = "PrecM";
        string HT = "HT";
        string Sense = "Sense";
        string dmg = "Dmg";

        #endregion
        string ver = "V0.313";
        //drone transmission settings
        int t_lim = 5;
        int nr_lim = 5;
        int nr_lim2 = 120;
        string D_I_N = "";
        string D_C_N = "";
        string dk_tsk_n = "";
        string undk_tsk_n = "";
        string Thr_ON_n = "";
        string Thr_OFF_N = "";
        string Rst_T_N = "";
        string CA_T_N = "";
        string P_M_T_N = "";
        string H_T_N = "";
        string D_S_C = "";
        string DLT = "";
        string P_CH = "";
        string p_cht = "ping";
        string drone_damage_status = "OK";
        string drone_output_status = "Idle";
        string recall_command = "recall";
        double trm_prec = 0.0;
        double trm_coeff = 0.04;
        float GyrMlt = 2;
        string dat_in;
        string dat_in2 = "";
        string dat_in3 = "";
        string dat_in4 = "";
        bool pinged = false;
        string dat_out;
        int drone_status = 0;
        int command_request = 0;
        int cmd_rqold = 0;
        bool mode_set = false;
        string drnst;
        string cmd_rqt = "0";
        string cmd_dist = "10.0";
        double no_cnvy_dst = 0.0;
        double tgtX = 0.0;
        double tgtY = 0.0;
        double tgtZ = 0.0;
        //comms channel
        string rx_ch;
        string tx_ch = "";
        string rx_channel_recall = "";
        string rx_channel_recall_drone = "";
        //logic flags
        float ttl_volu = 0.0f;
        float ttl_volm = 0.0f;
        float total_percent_cargo_used = 0.0f;
        float ttl_volus = 0.0f;
        float ttl_volms = 0.0f;
        float ttl_pctus = 0.0f;
        float ttl_PWRs;
        float ttl_sPWR;
        float ttl_PWRm;
        float ttl_mPWR;
        float ttl_cPWR;
        float ttl_PWRc;
        bool can_gyroOVR = false;
        bool trgt_vld = false;
        bool cnvyrsON = false;
        bool exit_WPadj = false;
        double TrgtPitch = 0.0;
        double TrgtRoll = 0.0;
        double TrgtYaw = 0.0;
        double ttl_GASs;
        double ttl_sGAS;
        double ttl_mGAS;
        double ttl_GASm;
        bool sens_convOPN = false;
        bool force_request_dock = false;
        bool request_exit = false;
        bool cmd_chng = false;
        bool stop_state = true;
        bool mine_state = false;
        bool nav_state = false;
        bool dock_state = false;
        bool undock_state = false;
        bool is_full = false;
        bool cargo_is_empty = false;
        bool cargo_full_achieved = false;
        bool is_full_charge = false;
        bool is_low_charge = false;
        bool recharge_request = false;
        bool was_mining = false;
        bool is_docking = false;
        bool is_undocking = false;
        bool is_autopiloting = false;
        bool is_docked = false;
        bool is_undocked = false;
        bool reset_mining = false;
        bool clr_cords = false;
        int cstm_dat_rd = 0;
        bool dat_invalid = false;
        bool dat_valid = false;
        bool tunnel_sequence_finished = false;
        int undocking_start = 0;
        bool is_full_tank = false;
        bool is_low_tank = false;
        bool recharge_request_tank = false;
        bool recharge_request_battery = false;
        int t_count = 0;
        int ns_count = 0;
        int ns_c2 = 0;
        bool nsr = false;
        bool t_delay = false;
        bool recall = false;
        bool dt_prsnt4 = false;
        bool dt_prsnt5 = false;
        bool dt_prsnt6 = false;
        string gps_dat_7 = "";
        string gps_dat_8 = "";
        string gps_dat_9 = "";
        string gps_dat_10 = "";
        string gps_dat_11 = "";
        string gps_dat_12 = "";
        string gpsindx = "";
        int cmd_read_ack = 0;
        int main_nav_sequence = 0;
        bool add_nav_Waypoint_mn = false;
        bool main_nav_complete = false;
        IMyRemoteControl rc_actual;
        IMyCameraBlock camera_actual;
        IMyShipConnector connector_actual;
        IMyRadioAntenna at_act;
        IMyGyro gyro_monitor;
        IMyTimerBlock tb_TON_act;
        IMyTimerBlock tb_TOFF_act;
        IMyPathRecorderBlock ai_dck_act;
        IMyPathRecorderBlock ai_task_undock_actual;
        IMyFlightMovementBlock ai_move_actual;
        IMyBatteryBlock crntbatteryblock;
        IMyLightingBlock dock_light_actual;
        IMyLightingBlock undock_light_actual;
        IMyLightingBlock collision_avoid_light_actual;
        IMyLightingBlock precM_light_actual;
        IMyLightingBlock reset_light_actual;
        IMyLightingBlock dmg_light_actual;
        IMySensorBlock sensor_actual;
        IMyGasTank crnthyrdogentank;
        Vector3D main_gps_coords;
        Vector3D mining_gps_coords;
        Vector3D mining_gps_coords_temp;
        Vector3D tgt_drill_start;
        Vector3D tgt_drill_end;
        Vector3D tgt_drill_exit;
        Vector3D exit_gps_coords_temp;
        Vector3D Last_Coords_Term;
        Vector3D crnt_tgt_align;
        Vector3D align_tgt_new;
        Vector3D directionb;
        Vector3D direction;
        Vector3D directionc;
        Vector3D gravity;
        bool r_delay = false;
        bool distance_id = false;
        bool exit_waypoint_set = false;
        bool exit_sequence_complete = false;
        bool mining_nav_complete = false;
        bool target_depth_achived = false;
        bool mine_coords_adjusted = false;
        bool add_mine_waypoint = false;
        int mining_stage = 0;
        int docking_stage = 0;
        int undocking_stage = 0;
        bool yawinst = false;
        bool pitchinst = false;
        bool rollinst = false;
        bool navinst = false;
        double distance_current = 0;
        bool nav_act = false;
        string ab0 = "ActivateBehavior_Off";
        string ab1 = "ActivateBehavior_On";
        string cc = "Connectable";
        string p1 = "ID_PLAY_CHECKBOX";
        List<IMyRemoteControl> rc_all;
        List<IMyRemoteControl> rctag;
        List<IMySensorBlock> sensor_all;
        List<IMySensorBlock> sensor_tag;
        List<IMyCameraBlock> cam_all;
        List<IMyCameraBlock> camera_tag;
        List<IMyShipConnector> connector_all;
        List<IMyShipConnector> connector_tag;
        List<IMyCargoContainer> cargo_all;
        List<IMyCargoContainer> cargo_tag;
        List<IMyCargoContainer> cargo_sense;
        List<IMyRadioAntenna> antenna_all;
        List<IMyRadioAntenna> antenna_tag;
        List<IMyPathRecorderBlock> flight_path_all;
        List<IMyPathRecorderBlock> flight_path_dock_tag;
        List<IMyPathRecorderBlock> flight_path_undock_tag;
        List<IMyFlightMovementBlock> flight_move_all;
        List<IMyFlightMovementBlock> flight_move_tag;
        List<IMyTimerBlock> timer_block_all;
        List<IMyTimerBlock> timer_block_tON_tag;
        List<IMyTimerBlock> timer_block_tOFF_tag;
        List<IMyTimerBlock> timer_block_precM_tag;
        List<IMyTimerBlock> timer_block_undock_tag;
        List<IMyLightingBlock> light_all;
        List<IMyLightingBlock> light_undock_tag;
        List<IMyLightingBlock> light_dock_tag;
        List<IMyLightingBlock> light_collision_avoid_tag;
        List<IMyLightingBlock> light_precM_tag;
        List<IMyLightingBlock> light_reset_tag;
        List<IMyLightingBlock> light_dmg_tag;
        List<IMyBatteryBlock> battery_all;
        List<IMyBatteryBlock> battery_tag;
        List<IMyGasTank> hydrogen_tank_all;
        List<IMyGasTank> hydrogen_tank_tag;
        List<IMyShipDrill> drill_all;
        List<IMyShipDrill> drill_tag;
        List<MyWaypointInfo> waypoints;
        List<IMyThrust> thrust_all;
        List<IMyThrust> thrust_tag;
        IMyGyro gyro_actual;
        List<IMyGyro> gyro_all;
        List<IMyGyro> gyro_tag;
        IMyShipDrill drl_act;
        StringBuilder sb;
        MyIni _ini = new MyIni();
        bool setup_complete = false;
        double gspeed = 0.0;
        string n = "";
        bool Or_recall_1 = false;
        bool Or_recall_2 = false;

        public void Save()
        {
            _ini.Set("commands", "c1", recall);
            _ini.Set("commands", "c2", stop_state);
            _ini.Set("commands", "c3", was_mining);
            _ini.Set("commands", "c4", nav_state);
            _ini.Set("commands", "c5", mine_state);
            _ini.Set("commands", "c6", dock_state);
            _ini.Set("commands", "c7", mode_set);
            _ini.Set("dockmode", "d1", docking_stage);
            _ini.Set("dockmode", "d2", undock_state);
            _ini.Set("dockmode", "d3", undocking_start);
            _ini.Set("dockmode", "d4", undocking_stage);
            _ini.Set("unitstate", "u1", recharge_request);
            _ini.Set("unitstate", "u2", nav_act);
            _ini.Set("unitstate", "u3", main_nav_sequence);
            _ini.Set("unitstate", "u4", main_nav_complete);
            _ini.Set("unitstate", "u5", add_nav_Waypoint_mn);
            _ini.Set("unitstate", "u6", distance_id);
            _ini.Set("unitstate", "u7", mining_stage);
            _ini.Set("unitstate", "u8", add_mine_waypoint);
            _ini.Set("unitstate", "u9", mine_coords_adjusted);
            _ini.Set("unitstate", "u10", target_depth_achived);
            _ini.Set("unitstate", "u11", reset_mining);
            _ini.Set("unitstate", "u12", mining_nav_complete);
            _ini.Set("unitstate", "u13", force_request_dock);
            _ini.Set("unitstate", "u14", request_exit);
            _ini.Set("unitstate", "u15", exit_sequence_complete);
            _ini.Set("unitstate", "u16", exit_waypoint_set);
            _ini.Set("unitstate", "u17", tunnel_sequence_finished);
            _ini.Set("unitstate", "u18", yawinst);
            _ini.Set("unitstate", "u19", pitchinst);
            _ini.Set("unitstate", "u20", rollinst);
            _ini.Set("unitstate", "u21", navinst);
            _ini.Set("unitstate", "u22", distance_current);
            _ini.Set("coordinates", "co1", mining_gps_coords.ToString());
            _ini.Set("coordinates", "co2", mining_gps_coords_temp.ToString());
            _ini.Set("coordinates", "co3", tgt_drill_start.ToString());
            _ini.Set("coordinates", "co4", tgt_drill_end.ToString());
            _ini.Set("coordinates", "co5", tgt_drill_exit.ToString());
            _ini.Set("coordinates", "co6", exit_gps_coords_temp.ToString());
            _ini.Set("coordinates", "co7", main_gps_coords.ToString());
            _ini.Set("coordinates", "co8", crnt_tgt_align.ToString());
            _ini.Set("coordinates", "co9", align_tgt_new.ToString());
            _ini.Set("coordinates", "co10", directionb.ToString());
            _ini.Set("coordinates", "co11", direction.ToString());
            _ini.Set("coordinates", "co12", directionc.ToString());
            _ini.Set("coordinates", "co13", gravity.ToString());
            _ini.Set("coordinates", "co14", gpsindx.ToString());
            Storage = _ini.ToString();
        }
        void Main(string argument)
        {
            IMyGridTerminalSystem gts = GridTerminalSystem as IMyGridTerminalSystem;
            if (!setup_complete)
            {
                sb = new StringBuilder();
                tx_ch = dtag + " reply";
                rx_channel_recall = dtag + " " + recall_command;
                if (dtag == "" || dtag == null)
                {
                    Echo("Invalid name for dtag");
                    return;
                }
                D_I_N = "[" + dtag + " " + drone_id_num + "]";
                D_C_N = "[" + dtag + " " + drone_id_num + "]";
                dk_tsk_n = "[" + dtag + " " + drone_id_num + " " + Dock + "]";
                undk_tsk_n = "[" + dtag + " " + drone_id_num + " " + Undock + "]";
                Thr_ON_n = "[" + dtag + " " + drone_id_num + " " + TON + "]";
                Thr_OFF_N = "[" + dtag + " " + drone_id_num + " " + TOFF + "]";
                Rst_T_N = "[" + dtag + " " + drone_id_num + " " + Reset + "]";
                CA_T_N = "[" + dtag + " " + drone_id_num + " " + CA + "]";
                P_M_T_N = "[" + dtag + " " + drone_id_num + " " + PrecM + "]";
                H_T_N = "[" + dtag + " " + drone_id_num + " " + HT + "]";
                D_S_C = "[" + dtag + " " + drone_id_num + " " + Sense + "]";
                DLT = "[" + dtag + " " + drone_id_num + " " + dmg + "]";
                P_CH = "[" + dtag + "]" + " " + p_cht;
                rx_channel_recall_drone = D_I_N + " " + recall_command;
                rc_all = new List<IMyRemoteControl>();
                rctag = new List<IMyRemoteControl>();
                gts.GetBlocksOfType<IMyRemoteControl>(rc_all, b => b.CubeGrid == Me.CubeGrid);
                if (rc_all.Count > 0)
                {
                    for (int i = 0; i < rc_all.Count; i++)
                    {
                        if (rc_all[i].CustomName.Contains(D_I_N))
                        {
                            rctag.Add(rc_all[i]);
                        }
                        if (!rc_all[i].CustomName.Contains(D_I_N))
                        {
                            n = rc_all[i].CustomName;
                            rc_all[i].CustomName = n + " " + D_I_N;
                            rctag.Add(rc_all[i]);
                        }

                    }
                }
                if (Collision_sense_enabled)
                {
                    sensor_all = new List<IMySensorBlock>();
                    sensor_tag = new List<IMySensorBlock>();
                    gts.GetBlocksOfType<IMySensorBlock>(sensor_all, b => b.CubeGrid == Me.CubeGrid);
                    if (sensor_all.Count > 0)
                    {
                        for (int i = 0; i < sensor_all.Count; i++)
                        {
                            if (sensor_all[i].CustomName.Contains(D_I_N))
                            {
                                sensor_tag.Add(sensor_all[i]);
                            }
                            if (!sensor_all[i].CustomName.Contains(D_I_N))
                            {
                                n = sensor_all[i].CustomName;
                                sensor_all[i].CustomName = n + " " + D_I_N;
                                sensor_tag.Add(sensor_all[i]);
                            }
                        }
                    }
                    if (sensor_tag.Count <= 0 || sensor_tag[0] == null)
                    {
                        Echo($"Sensor with tag: '{D_I_N}' not found.");
                        return;
                    }
                    sensor_actual = sensor_tag[0];
                    sensor_actual.DetectAsteroids = true;
                    sensor_actual.DetectEnemy = true;
                    sensor_actual.DetectFriendly = true;
                    sensor_actual.DetectLargeShips = true;
                    sensor_actual.DetectSmallShips = true;
                    sensor_actual.DetectSubgrids = true;
                    sensor_actual.DetectFloatingObjects = false;
                    sensor_actual.DetectStations = true;
                    sensor_actual.DetectPlayers = false;
                    sensor_actual.DetectNeutral = true;
                    sensor_actual.DetectOwner = true;
                    sensor_actual.LeftExtend = s_llm;
                    sensor_actual.RightExtend = s_rlm;
                    sensor_actual.BottomExtend = s_btlm;
                    sensor_actual.TopExtend = s_tlm;
                    sensor_actual.BackExtend = s_bklm;
                    sensor_actual.FrontExtend = s_flm;
                }
                cam_all = new List<IMyCameraBlock>();
                camera_tag = new List<IMyCameraBlock>();
                gts.GetBlocksOfType<IMyCameraBlock>(cam_all, b => b.CubeGrid == Me.CubeGrid);
                if (cam_all.Count > 0)
                {
                    for (int i = 0; i < cam_all.Count; i++)
                    {
                        if (cam_all[i].CustomName.Contains(D_I_N))
                        {
                            camera_tag.Add(cam_all[i]);
                        }
                        if (!cam_all[i].CustomName.Contains(D_I_N))
                        {
                            n = cam_all[i].CustomName;
                            cam_all[i].CustomName = n + " " + D_I_N;
                            camera_tag.Add(cam_all[i]);
                        }
                    }
                }
                connector_all = new List<IMyShipConnector>();
                connector_tag = new List<IMyShipConnector>();
                gts.GetBlocksOfType<IMyShipConnector>(connector_all, b => b.CubeGrid == Me.CubeGrid);
                if (connector_all.Count > 0)
                {
                    for (int i = 0; i < connector_all.Count; i++)
                    {
                        if (connector_all[i].CustomName.Contains(D_C_N))
                        {
                            connector_tag.Add(connector_all[i]);
                        }
                        if (!connector_all[i].CustomName.Contains(D_I_N))
                        {
                            n = connector_all[i].CustomName;
                            connector_all[i].CustomName = n + " " + D_I_N;
                            connector_tag.Add(connector_all[i]);
                        }
                    }
                }
                cargo_all = new List<IMyCargoContainer>();
                cargo_tag = new List<IMyCargoContainer>();
                cargo_sense = new List<IMyCargoContainer>();
                gts.GetBlocksOfType<IMyCargoContainer>(cargo_all, b => b.CubeGrid == Me.CubeGrid);
                if (cargo_all.Count > 0)
                {
                    for (int i = 0; i < cargo_all.Count; i++)
                    {
                        if (cargo_all[i].CustomName.Contains(D_I_N))
                        {
                            cargo_tag.Add(cargo_all[i]);
                        }
                        if (cargo_all[i].CustomName.Contains(D_S_C))
                        {
                            cargo_sense.Add(cargo_all[i]);
                        }
                        if (!cargo_all[i].CustomName.Contains(D_I_N) && !cargo_all[i].CustomName.Contains(D_S_C))
                        {
                            n = cargo_all[i].CustomName;
                            cargo_all[i].CustomName = n + " " + D_I_N;
                            cargo_tag.Add(cargo_all[i]);
                        }
                    }
                }
                antenna_all = new List<IMyRadioAntenna>();
                antenna_tag = new List<IMyRadioAntenna>();
                gts.GetBlocksOfType<IMyRadioAntenna>(antenna_all, b => b.CubeGrid == Me.CubeGrid);
                if (antenna_all.Count > 0)
                {
                    for (int i = 0; i < antenna_all.Count; i++)
                    {
                        if (antenna_all[i].CustomName.Contains(D_I_N))
                        {
                            antenna_tag.Add(antenna_all[i]);
                        }
                        if (!antenna_all[i].CustomName.Contains(D_I_N))
                        {
                            n = antenna_all[i].CustomName;
                            antenna_all[i].CustomName = n + " " + D_I_N;
                            antenna_tag.Add(antenna_all[i]);
                        }
                    }
                }
                flight_path_all = new List<IMyPathRecorderBlock>();
                flight_path_dock_tag = new List<IMyPathRecorderBlock>();
                flight_path_undock_tag = new List<IMyPathRecorderBlock>();
                gts.GetBlocksOfType<IMyPathRecorderBlock>(flight_path_all, b => b.CubeGrid == Me.CubeGrid);
                if (flight_path_all.Count > 0)
                {
                    for (int i = 0; i < flight_path_all.Count; i++)
                    {
                        if (flight_path_all[i].CustomName.Contains(dk_tsk_n))
                        {
                            flight_path_dock_tag.Add(flight_path_all[i]);
                        }
                        if (flight_path_all[i].CustomName.Contains(undk_tsk_n))
                        {
                            flight_path_undock_tag.Add(flight_path_all[i]);
                        }
                    }
                }
                flight_move_all = new List<IMyFlightMovementBlock>();
                flight_move_tag = new List<IMyFlightMovementBlock>();
                gts.GetBlocksOfType<IMyFlightMovementBlock>(flight_move_all, b => b.CubeGrid == Me.CubeGrid);
                if (flight_move_all.Count > 0)
                {
                    for (int i = 0; i < flight_move_all.Count; i++)
                    {
                        if (flight_move_all[i].CustomName.Contains(D_I_N))
                        {
                            flight_move_tag.Add(flight_move_all[i]);
                        }
                        if (!flight_move_all[i].CustomName.Contains(D_I_N))
                        {
                            n = flight_move_all[i].CustomName;
                            flight_move_all[i].CustomName = n + " " + D_I_N;
                            flight_move_tag.Add(flight_move_all[i]);
                        }
                    }
                }
                thrust_all = new List<IMyThrust>();
                thrust_tag = new List<IMyThrust>();
                gts.GetBlocksOfType<IMyThrust>(thrust_all, b => b.CubeGrid == Me.CubeGrid);
                if (thrust_all.Count > 0)
                {
                    for (int i = 0; i < thrust_all.Count; i++)
                    {
                        if (thrust_all[i].CustomName.Contains(D_I_N))
                        {
                            thrust_tag.Add(thrust_all[i]);
                        }
                        if (!thrust_all[i].CustomName.Contains(D_I_N))
                        {
                            n = thrust_all[i].CustomName;
                            thrust_all[i].CustomName = n + " " + D_I_N;
                            thrust_tag.Add(thrust_all[i]);
                        }
                    }
                }
                timer_block_all = new List<IMyTimerBlock>();
                timer_block_tON_tag = new List<IMyTimerBlock>();
                timer_block_tOFF_tag = new List<IMyTimerBlock>();
                timer_block_precM_tag = new List<IMyTimerBlock>();
                timer_block_undock_tag = new List<IMyTimerBlock>();
                gts.GetBlocksOfType<IMyTimerBlock>(timer_block_all, b => b.CubeGrid == Me.CubeGrid);
                if (timer_block_all.Count > 0)
                {
                    for (int i = 0; i < timer_block_all.Count; i++)
                    {
                        if (timer_block_all[i].CustomName.Contains(Thr_ON_n))
                        {
                            timer_block_tON_tag.Add(timer_block_all[i]);
                        }
                        if (timer_block_all[i].CustomName.Contains(Thr_OFF_N))
                        {
                            timer_block_tOFF_tag.Add(timer_block_all[i]);
                        }
                        if (timer_block_all[i].CustomName.Contains(P_M_T_N))
                        {
                            timer_block_precM_tag.Add(timer_block_all[i]);
                        }
                        if (timer_block_all[i].CustomName.Contains(undk_tsk_n))
                        {
                            timer_block_undock_tag.Add(timer_block_all[i]);
                        }
                    }
                }
                light_all = new List<IMyLightingBlock>();
                light_undock_tag = new List<IMyLightingBlock>();
                light_dock_tag = new List<IMyLightingBlock>();
                light_collision_avoid_tag = new List<IMyLightingBlock>();
                light_precM_tag = new List<IMyLightingBlock>();
                light_reset_tag = new List<IMyLightingBlock>();
                light_dmg_tag = new List<IMyLightingBlock>();
                gts.GetBlocksOfType<IMyLightingBlock>(light_all, b => b.CubeGrid == Me.CubeGrid);
                if (light_all.Count > 0)
                {
                    for (int i = 0; i < light_all.Count; i++)
                    {
                        if (light_all[i].CustomName.Contains(dk_tsk_n))
                        {
                            light_dock_tag.Add(light_all[i]);
                        }
                        if (light_all[i].CustomName.Contains(undk_tsk_n))
                        {
                            light_undock_tag.Add(light_all[i]);
                        }
                        if (light_all[i].CustomName.Contains(CA_T_N))
                        {
                            light_collision_avoid_tag.Add(light_all[i]);
                        }
                        if (light_all[i].CustomName.Contains(Rst_T_N))
                        {
                            light_reset_tag.Add(light_all[i]);
                        }
                        if (light_all[i].CustomName.Contains(P_M_T_N))
                        {
                            light_precM_tag.Add(light_all[i]);
                        }
                        if (light_all[i].CustomName.Contains(DLT))
                        {
                            light_dmg_tag.Add(light_all[i]);
                        }
                    }
                }
                battery_all = new List<IMyBatteryBlock>();
                battery_tag = new List<IMyBatteryBlock>();
                gts.GetBlocksOfType<IMyBatteryBlock>(battery_all, b => b.CubeGrid == Me.CubeGrid);
                if (battery_all.Count > 0)
                {
                    for (int i = 0; i < battery_all.Count; i++)
                    {
                        if (battery_all[i].CustomName.Contains(D_I_N))
                        {
                            battery_tag.Add(battery_all[i]);
                        }
                        if (!battery_all[i].CustomName.Contains(D_I_N))
                        {
                            n = battery_all[i].CustomName;
                            battery_all[i].CustomName = n + " " + D_I_N;
                            battery_tag.Add(battery_all[i]);
                        }
                    }
                }
                hydrogen_tank_all = new List<IMyGasTank>();
                hydrogen_tank_tag = new List<IMyGasTank>();
                gts.GetBlocksOfType<IMyGasTank>(hydrogen_tank_all, b => b.CubeGrid == Me.CubeGrid);
                if (hydrogen_tank_all.Count > 0)
                {
                    for (int i = 0; i < hydrogen_tank_all.Count; i++)
                    {
                        if (hydrogen_tank_all[i].CustomName.Contains(H_T_N))
                        {
                            hydrogen_tank_tag.Add(hydrogen_tank_all[i]);
                        }
                        if (!hydrogen_tank_all[i].CustomName.Contains(H_T_N))
                        {
                            n = hydrogen_tank_all[i].CustomName;
                            hydrogen_tank_all[i].CustomName = n + " " + H_T_N;
                            hydrogen_tank_tag.Add(hydrogen_tank_all[i]);
                        }
                    }
                }
                drill_all = new List<IMyShipDrill>();
                drill_tag = new List<IMyShipDrill>();
                gts.GetBlocksOfType<IMyShipDrill>(drill_all, b => b.CubeGrid == Me.CubeGrid);
                if (drill_all.Count > 0)
                {
                    for (int i = 0; i < drill_all.Count; i++)
                    {
                        if (drill_all[i].CustomName.Contains(D_I_N))
                        {
                            drill_tag.Add(drill_all[i]);
                        }
                        if (!drill_all[i].CustomName.Contains(D_I_N))
                        {
                            n = drill_all[i].CustomName;
                            drill_all[i].CustomName = n + " " + D_I_N;
                            drill_tag.Add(drill_all[i]);
                        }
                    }
                }
                gyro_all = new List<IMyGyro>();
                gyro_tag = new List<IMyGyro>();
                gts.GetBlocksOfType<IMyGyro>(gyro_all, b => b.CubeGrid == Me.CubeGrid);
                if (gyro_all.Count > 0)
                {
                    for (int i = 0; i < gyro_all.Count; i++)
                    {
                        if (gyro_all[i].CustomName.Contains(D_I_N))
                        {
                            gyro_tag.Add(gyro_all[i]);
                        }
                        if (!gyro_all[i].CustomName.Contains(D_I_N))
                        {
                            n = gyro_all[i].CustomName;
                            gyro_all[i].CustomName = n + " " + D_I_N;
                            gyro_tag.Add(gyro_all[i]);
                        }
                    }
                }
                if (Storage != "" && Storage != null)
                {
                    LoadStorageData();
                    Storage = "";
                }
                setup_complete = true;
                Echo("Setup complete!");
            }
            rx_ch = D_I_N;
            IMyBroadcastListener listn = IGC.RegisterBroadcastListener(rx_ch);
            IMyBroadcastListener listn_recall = IGC.RegisterBroadcastListener(rx_channel_recall);
            IMyBroadcastListener listn_recall_drone = IGC.RegisterBroadcastListener(rx_channel_recall_drone);
            IMyBroadcastListener listn_png = IGC.RegisterBroadcastListener(P_CH);

            waypoints = new List<MyWaypointInfo>();
            if (drill_tag.Count <= 0)
            {
                Echo($"Drills with tag: '{D_I_N}' not found.");
                return;
            }
            if (gyro_tag.Count <= 0 || gyro_tag[0] == null)
            {
                Echo($"Gyro with tag: '{D_I_N}' not found.");
                return;
            }
            if (rctag.Count <= 0 || rctag[0] == null)
            {
                Echo($"Remote control with tag: '{D_I_N}' not found.");
                return;
            }
            rc_actual = rctag[0];
            if (Collision_sense_enabled)
            {
                if (sensor_tag.Count <= 0 || sensor_tag[0] == null)
                {
                    Echo($"Sensor with tag: '{D_I_N}' not found.");
                    return;
                }
                sensor_actual = sensor_tag[0];
            }

            if (camera_tag.Count <= 0 || camera_tag[0] == null)
            {
                Echo($"Camera with tag: '{D_I_N}' not found.");
                return;
            }
            camera_actual = camera_tag[0];
            if (connector_tag.Count <= 0 || connector_tag[0] == null)
            {
                Echo($"Connector with tag: '{D_C_N}' not found.");
                return;
            }
            connector_actual = connector_tag[0];
            if (cargo_tag.Count <= 0 || cargo_tag[0] == null)
            {
                Echo($"Cargo containers with tag: '{D_I_N}' not found.");
                return;
            }
            if (ORE_sense_enabled)
            {
                if (cargo_sense.Count <= 0 || cargo_sense[0] == null)
                {
                    Echo($"Sense container with tag: '{D_S_C}' not found.");
                    return;
                }
            }
            ttl_volu = 0;
            ttl_volm = 0;
            total_percent_cargo_used = 0;
            for (int i = 0; i < cargo_tag.Count; i++)
            {
                float inventory_vol = (float)cargo_tag[i].GetInventory(0).CurrentVolume;
                float max_inventory_vol = (float)cargo_tag[i].GetInventory(0).MaxVolume;
                ttl_volu = ttl_volu + inventory_vol;
                ttl_volm = ttl_volm + max_inventory_vol;
                total_percent_cargo_used = (ttl_volu / ttl_volm) * 100;
            }
            if (total_percent_cargo_used == 100.0f)
            {
                is_full = true;
            }
            if (total_percent_cargo_used < 100.0f)
            {
                is_full = false;
            }
            if (total_percent_cargo_used == 0.0f)
            {
                cargo_is_empty = true;
            }
            if (total_percent_cargo_used > 0.0f)
            {
                cargo_is_empty = false;
            }
            if (cargo_is_empty && cargo_full_achieved)
            {
                cargo_full_achieved = false;
            }
            if (ORE_sense_enabled)
            {
                ttl_volus = 0;
                ttl_volms = 0;
                ttl_pctus = 0;
                for (int i = 0; i < cargo_sense.Count; i++)
                {
                    float inventory_vol_s = (float)cargo_sense[i].GetInventory(0).CurrentVolume;
                    float max_inventory_vol_s = (float)cargo_sense[i].GetInventory(0).MaxVolume;
                    ttl_volus = ttl_volus + inventory_vol_s;
                    ttl_volms = ttl_volms + max_inventory_vol_s;
                    ttl_pctus = (ttl_volus / ttl_volms) * 100;
                }

                if (ttl_pctus > ORE_sense_limit)
                {
                    sens_convOPN = true;
                }
                else
                {
                    sens_convOPN = false;
                }
            }
            if (antenna_tag.Count <= 0 || antenna_tag[0] == null)
            {
                Echo($"Antenna with tag: '{D_I_N}' not found.");
                return;
            }
            at_act = antenna_tag[0];
            if (flight_path_dock_tag.Count <= 0 || flight_path_dock_tag[0] == null)
            {
                Echo($"Docking AI task recorder with tag: '{dk_tsk_n}' not found.");
                return;
            }
            ai_dck_act = flight_path_dock_tag[0];
            if (flight_path_undock_tag.Count <= 0 || flight_path_undock_tag[0] == null)
            {
                Echo($"Undocking AI task recorder with tag: '{undk_tsk_n}' not found.");
                return;
            }
            ai_task_undock_actual = flight_path_undock_tag[0];
            if (flight_move_tag.Count <= 0 || flight_move_tag[0] == null)
            {
                Echo($"Flight movement with tag: '{D_I_N}' not found.");
                return;
            }
            ai_move_actual = flight_move_tag[0];
            if (timer_block_tON_tag.Count <= 0 || timer_block_tON_tag[0] == null)
            {
                Echo($"Thrust ON timer block with tag: '{Thr_ON_n}' not found.");
                return;
            }
            tb_TON_act = timer_block_tON_tag[0];
            if (timer_block_tOFF_tag.Count <= 0 || timer_block_tOFF_tag[0] == null)
            {
                Echo($"Thrust OFF timer block with tag: '{Thr_OFF_N}' not found.");
                return;
            }
            tb_TOFF_act = timer_block_tOFF_tag[0];
            if (timer_block_precM_tag.Count <= 0 || timer_block_precM_tag[0] == null)
            {
                Echo($"Precision mode timer block with tag: '{P_M_T_N}' not found.");
                return;
            }
            if (timer_block_undock_tag.Count <= 0 || timer_block_undock_tag[0] == null)
            {
                Echo($"Undock mode timer block with tag: '{undk_tsk_n}' not found.");
                return;
            }
            if (light_dock_tag.Count <= 0 || light_dock_tag[0] == null)
            {
                Echo($"dock indicator light with tag: '{dk_tsk_n}' not found.");
                return;
            }
            dock_light_actual = light_dock_tag[0];

            if (light_undock_tag.Count <= 0 || light_undock_tag[0] == null)
            {
                Echo($"undock indicator light with tag: '{undk_tsk_n}' not found.");
                return;
            }
            undock_light_actual = light_undock_tag[0];
            if (light_collision_avoid_tag.Count <= 0 || light_collision_avoid_tag[0] == null)
            {
                Echo($"collision avoidance required indicator light with tag: '{CA_T_N}' not found.");
                return;
            }
            collision_avoid_light_actual = light_collision_avoid_tag[0];
            if (light_precM_tag.Count <= 0 || light_precM_tag[0] == null)
            {
                Echo($"Precision mode required indicator light with tag: '{P_M_T_N}' not found.");
                return;
            }

            precM_light_actual = light_precM_tag[0];
            if (light_reset_tag.Count <= 0 || light_reset_tag[0] == null)
            {
                Echo($"Dock reset indicator light with tag: '{Rst_T_N}' not found.");
                return;
            }
            reset_light_actual = light_reset_tag[0];

            if (light_dmg_tag.Count <= 0 && dmg_report_enabled)
            {
                Echo($"Damage indicator light with tag: '{DLT}' not found.");
                Echo("");
            }
            if (light_dmg_tag.Count > 0 && dmg_report_enabled && light_dmg_tag[0] != null)
            {
                dmg_light_actual = light_dmg_tag[0];
            }

            if (battery_tag.Count <= 0 || battery_tag[0] == null)
            {
                Echo($"Batteries with tag: '{D_I_N}' not found.");
                return;
            }
            if (!dmg_report_enabled)
            {
                drone_damage_status = "OK";
            }
            if (dmg_report_enabled)
            {
                if (dmg_light_actual == null && dmg_report_enabled)
                {
                    drone_damage_status = "UNK";
                }
                if (dmg_light_actual != null)
                {
                    if (dmg_light_actual.Enabled && dmg_report_enabled && dmg_light_actual.IsFunctional || dmg_report_enabled && !dmg_light_actual.IsFunctional)
                    {
                        drone_damage_status = "DMG";
                    }
                    if (!dmg_light_actual.Enabled && dmg_report_enabled && dmg_light_actual.IsFunctional)
                    {
                        drone_damage_status = "OK";
                    }
                }
            }
            ttl_PWRs = 0;
            ttl_sPWR = 0;
            ttl_PWRm = 0;
            ttl_mPWR = 0;
            ttl_cPWR = 0;
            ttl_PWRc = 0;
            float percent_battery_power = 0.0f;
            for (int i = 0; i < battery_tag.Count; i++)
            {
                crntbatteryblock = battery_tag[i];
                ttl_PWRs = crntbatteryblock.CurrentStoredPower;
                ttl_sPWR = ttl_sPWR + ttl_PWRs;
                ttl_PWRm = crntbatteryblock.MaxStoredPower;
                ttl_mPWR = ttl_mPWR + ttl_PWRm;
                ttl_cPWR = crntbatteryblock.CurrentOutput;
                ttl_PWRc = ttl_PWRc + ttl_cPWR;
                percent_battery_power = (ttl_sPWR / ttl_mPWR) * 100;
            }
            if (percent_battery_power == bat_CHGhi)
            {
                is_full_charge = true;
            }
            if (percent_battery_power < bat_CHGhi)
            {
                is_full_charge = false;
            }
            if (percent_battery_power <= bat_CHGlow)
            {
                is_low_charge = true;
            }
            if (percent_battery_power > bat_CHGlow)
            {
                is_low_charge = false;
            }
            if (!is_low_charge && recharge_request_battery && is_full_charge)
            {
                recharge_request_battery = false;
            }
            if (!recharge_request_battery)
            {
                for (int i = 0; i < battery_tag.Count; i++)
                {
                    battery_tag[i].ChargeMode = ChargeMode.Auto;
                }
            }

            if (hydrogen_tank_tag.Count <= 0 || hydrogen_tank_tag[0] == null)
            {
            }
            ttl_GASs = 0;
            ttl_sGAS = 0;
            ttl_mGAS = 0;
            ttl_GASm = 0;
            double pcnt_gas_tank = 0.0;
            if (hydrogen_tank_tag.Count > 0)
            {
                for (int i = 0; i < hydrogen_tank_tag.Count; i++)
                {
                    crnthyrdogentank = hydrogen_tank_tag[i];
                    ttl_GASs = crnthyrdogentank.FilledRatio * 100.0f;
                    ttl_sGAS = ttl_sGAS + ttl_GASs;
                    ttl_mGAS = 100.0f;
                    ttl_GASm = ttl_GASm + ttl_mGAS;
                    pcnt_gas_tank = (ttl_sGAS / ttl_GASm) * 100.0f;
                }
            }
            if (pcnt_gas_tank == gas_CHGhi)
            {
                is_full_tank = true;
            }
            if (pcnt_gas_tank < gas_CHGhi)
            {
                is_full_tank = false;
            }
            if (pcnt_gas_tank <= gas_CHGlow)
            {
                is_low_tank = true;
            }
            if (pcnt_gas_tank > gas_CHGlow)
            {
                is_low_tank = false;
            }
            if (!is_low_tank && recharge_request_tank && is_full_tank && !ignore_Htank || ignore_Htank)
            {
                recharge_request_tank = false;
            }
            if (!recharge_request_tank && !ignore_Htank)
            {
                if (hydrogen_tank_tag.Count > 0)
                {
                    for (int i = 0; i < hydrogen_tank_tag.Count; i++)
                    {
                        hydrogen_tank_tag[i].Stockpile = false;
                    }
                }
            }
            if (recharge_request_battery || recharge_request_tank)
            {
                recharge_request = true;
            }
            else
            {
                recharge_request = false;
            }

            // ** Logic Start **
            Echo($"GMDS {ver} Running...");
            trm_prec = (trm_coeff * drill_sl) + 0.6;
            //comms
            if (listn.HasPendingMessage)
            {
                MyIGCMessage new_msg = listn.AcceptMessage();
                dat_in = new_msg.Data.ToString();
            }
            if (listn_recall.HasPendingMessage)
            {
                MyIGCMessage new_msg_2 = listn_recall.AcceptMessage();
                dat_in2 = new_msg_2.Data.ToString();
            }
            if (listn_recall_drone.HasPendingMessage)
            {
                MyIGCMessage new_msg_4 = listn_recall_drone.AcceptMessage();
                dat_in4 = new_msg_4.Data.ToString();
            }
            if (listn_png.HasPendingMessage)
            {
                MyIGCMessage new_msg_3 = listn_png.AcceptMessage();
                dat_in3 = new_msg_3.Data.ToString();
            }
            if (dat_in != null)
            {
                Me.CustomData = dat_in;
            }
            if (dat_in2 != null)
            {
                if (dat_in2.Contains(recall_command))
                {
                    Or_recall_1 = true;
                }
                else
                {
                    Or_recall_1 = false;
                }
            }
            if (dat_in4 != null)
            {
                if (dat_in4.Contains(recall_command))
                {
                    Or_recall_2 = true;
                }
                else
                {
                    Or_recall_2 = false;
                }
            }
            //recall management
            if (Or_recall_1 || Or_recall_2)
            {
                recall = true;
            }
            else
            {
                recall = false;
            }
            if (dat_in3 != null)
            {
                if (dat_in3.Contains(p_cht))
                {
                    pinged = true;
                }
                else
                {
                    pinged = false;
                }
            }
            if (Me.CustomData != null && Me.CustomData != "")
            {
                dat_valid = true;
            }
            else dat_valid = false;

            if (Me.CustomData == null || Me.CustomData == "")
            {
                dat_invalid = true;
            }
            else dat_invalid = false;
            if (dat_valid)
            {
                if (cstm_dat_rd == 1)
                {
                    cmd_rqold = command_request;
                    cstm_dat_rd = 0;
                    drone_status = 25;
                }
                if (cstm_dat_rd == 0)
                {
                    GetCustomData();
                    cstm_dat_rd = 1;
                    if (command_request != cmd_rqold)
                    {
                        cmd_chng = true;
                    }
                    if (command_request == cmd_rqold)
                    {
                        cmd_chng = false;
                    }
                    drone_status = 24;
                }
            }
            if (dat_invalid)
            {
                if (cstm_dat_rd == 1)
                {
                    cmd_rqold = command_request;
                    cstm_dat_rd = 0;
                    drone_status = 25;
                }
                if (cstm_dat_rd == 0)
                {
                    GetCustomData();
                    cstm_dat_rd = 1;
                    if (command_request != cmd_rqold)
                    {
                        cmd_chng = true;
                    }

                    if (command_request == cmd_rqold)
                    {
                        cmd_chng = false;
                    }
                    drone_status = 24;
                }
            }
            if (dat_invalid && !was_mining)
            {
                command_request = 0;
            }
            if (command_request == 0 || cmd_chng && mode_set && command_request != 7)
            {
                stop_state = true;
                cmd_read_ack = 0;
            }
            else stop_state = false;
            if (command_request == 0 && was_mining)
            {
                was_mining = false;
                target_depth_achived = false;
            }
            if (command_request == 0 && target_depth_achived)
            {
                target_depth_achived = false;
            }
            if (command_request >= 1 && command_request <= 4)
            {
                nav_state = true;
            }
            else nav_state = false;
            if (command_request == 5 && !request_exit)
            {
                mine_state = true;
            }
            else mine_state = false;
            if (command_request == 6)
            {
                dock_state = true;
            }
            else dock_state = false;
            if (command_request == 7 && !recall && !recharge_request)
            {
                undock_state = true;
            }
            else undock_state = false;
            if (command_request == 8 && tunnel_sequence_finished || command_request == 0 && connector_actual.IsConnected && tunnel_sequence_finished && !undock_state && !cargo_full_achieved && cargo_is_empty && !recharge_request)
            {
                tunnel_sequence_finished = false;
                drone_output_status = "Resetting";
            }
            if (connector_actual.IsConnected && auto_charge_mode && !recharge_request_battery && !undock_state || connector_actual.IsConnected && auto_charge_mode && recharge_request_battery && !undock_state)
            {
                for (int i = 0; i < battery_tag.Count; i++)
                {
                    battery_tag[i].ChargeMode = ChargeMode.Recharge;
                }
            }
            if (!connector_actual.IsConnected && auto_charge_mode && !recharge_request_battery || undock_state && auto_charge_mode && !recharge_request_battery)
            {
                for (int i = 0; i < battery_tag.Count; i++)
                {
                    battery_tag[i].ChargeMode = ChargeMode.Auto;
                }
            }
            if (undock_state && !recharge_request && cargo_is_empty && !cargo_full_achieved && !target_depth_achived && connector_actual.IsConnected && undocking_stage == 0 && !tb_TON_act.IsCountingDown)
            {
                if (!is_docking || !is_undocking)
                {
                    reset_ai();
                }
                undocking_start = 0;
                drone_output_status = "Undocking";
                dock_light_actual.Enabled = false;
                connector_actual.Enabled = false;
                if (!tb_TON_act.Enabled)
                {
                    tb_TON_act.Enabled = true;
                }
                tb_TON_act.Trigger();
                undocking_stage = 1;
            }
            if (undocking_stage == 1 && !connector_actual.IsConnected)
            {
                reset_ai();
                connector_actual.Enabled = false;
                collision_avoid_light_actual.Enabled = false;
                undock_light_actual.Enabled = false;
                ai_move_actual.PrecisionMode = true;
                ai_move_actual.CollisionAvoidance = false;
                ai_move_actual.GetActionWithName(ab1).Apply(ai_move_actual);
                ai_task_undock_actual.GetActionWithName(ab1).Apply(ai_task_undock_actual);
                ai_task_undock_actual.GetActionWithName(p1).Apply(ai_task_undock_actual);
                undocking_stage = 2;
            }

            if (undocking_stage == 2 && undock_light_actual.Enabled && !connector_actual.IsConnected)
            {
                collision_avoid_light_actual.Enabled = false;
                if (Collision_sense_enabled)
                {
                    sensor_actual.Enabled = false;
                }
                ai_move_actual.PrecisionMode = false;
                ai_move_actual.CollisionAvoidance = true;
                connector_actual.Enabled = true;
                undocking_stage = 3;
            }
            if (!connector_actual.IsConnected && !ai_task_undock_actual.GetValue<bool>(p1) && undocking_stage == 2 && !undock_light_actual.Enabled)
            {
                if (udock_conf)
                {
                    undocking_stage = 1;
                }
                if (!udock_conf)
                {
                    undock_light_actual.Enabled = true;
                }
            }

            if (is_undocked && undocking_stage == 3 && undocking_start == 0)
            {
                drone_output_status = "Undocked";
                reset_ai();
                undocking_start = 1;
            }
            if (dock_state && !connector_actual.IsConnected && docking_stage == 0)
            {
                drone_output_status = "Docking init";
                if (!is_docking || !is_undocking)
                {
                    reset_ai();
                }
                docking_stage = 1;
                main_nav_sequence = 0;
                collision_avoid_light_actual.Enabled = true;
                if (Collision_sense_enabled)
                {
                    sensor_actual.Enabled = true;
                }
                if (!undock_light_actual.Enabled)
                {
                    undock_light_actual.Enabled = true;
                }
                if (dock_light_actual.Enabled)
                {
                    dock_light_actual.Enabled = false;
                }
            }
            if (undocking_stage > 0 && undocking_stage < 3)
            {
                is_undocking = true;
                ns_c2++;
            }
            if (nsr && undocking_stage > 0 && undocking_stage < 3 && !connector_actual.IsConnected)
            {
                reset_light_actual.Enabled = true;
                undock_light_actual.Enabled = true;
                undocking_stage = 3;
            }

            if (undocking_stage > 2 && is_undocked)
            {
                is_undocking = false;
            }
            if (docking_stage > 0)
            {
                is_docking = true;
            }
            else is_docking = false;

            if (is_undocking || is_docking)
            {
                is_autopiloting = true;
            }
            else is_autopiloting = false;
            if (dock_light_actual.Enabled)
            {
                is_docked = true;
            }
            else is_docked = false;

            if (undock_light_actual.Enabled)
            {
                is_undocked = true;
            }
            else is_undocked = false;
            if (stop_state)
            {
                main_nav_sequence = 0;
                main_nav_complete = false;
                add_nav_Waypoint_mn = false;
                if (!was_mining)
                {
                    rc_actual.SetCollisionAvoidance(false);
                    rc_actual.SetDockingMode(false);
                    rc_actual.SetAutoPilotEnabled(false);
                    rc_actual.ClearWaypoints();
                    distance_id = false;
                    add_mine_waypoint = false;
                }

                if (!request_exit)
                {
                    mining_stage = 0;
                    mine_state = false;
                }
                nav_state = false;
                dock_state = false;
                drone_output_status = "Idle";
                undocking_stage = 0;
                drone_status = 0;
                cmd_rqt = "0";
            }
            if (mining_stage == 0 && stop_state && request_exit)
            {
                request_exit = false;
            }
            if (mine_state && !was_mining)
            {
                was_mining = true;
            }
            if (reset_mining && was_mining)
            {
                was_mining = false;
                reset_mining = false;
                mining_stage = 0;
            }
            if (reset_mining && !was_mining)
            {
                reset_mining = false;
            }
            if (!mine_state && !was_mining && mining_stage > 0)
            {
                mining_stage = 0;
            }
            if (mine_state || nav_state)
            {
                mode_set = true;
            }
            else mode_set = false;
            if (dat_invalid && target_depth_achived && !request_exit && was_mining && cstm_dat_rd == 1 && is_undocked)
            {
                request_exit = true;
            }
            if (target_depth_achived || cargo_full_achieved || recharge_request || recall)
            {
                force_request_dock = true;
            }
            else force_request_dock = false;

            gravity = rc_actual.GetNaturalGravity();
            if (trgt_vld)
            {
                crnt_tgt_align = align_tgt_new;
            }
            if (!trgt_vld)
            {
                crnt_tgt_align = gravity;
            }

            if (is_docked || docking_stage > 0 || !is_undocked && !is_docked)
            {
                can_gyroOVR = false;
            }
            else
            {
                can_gyroOVR = true;
            }
            if (mining_stage >= 6 && mining_stage <= 10)
            {
                can_gyroOVR = true;
            }
            SetGyroOverride(can_gyroOVR, GetNavAngles(crnt_tgt_align) * GyrMlt);
            double YawMon = GetNavAngles(crnt_tgt_align).GetDim(0);
            double PitchMon = GetNavAngles(crnt_tgt_align).GetDim(1);
            double RollMon = GetNavAngles(crnt_tgt_align).GetDim(2);

            if (YawMon > nav_inst_thr || YawMon < -nav_inst_thr)
            {
                drone_status = 9;
                yawinst = true;
            }
            else yawinst = false;

            if (PitchMon > nav_inst_thr || PitchMon < -nav_inst_thr)
            {
                pitchinst = true;
                drone_status = 9;
            }
            else pitchinst = false;
            if (RollMon > nav_inst_thr || RollMon < -nav_inst_thr)
            {
                rollinst = true;
                drone_status = 9;
            }
            else rollinst = false;
            if (main_nav_sequence > 0 && main_nav_sequence < 4 && trgt_vld)
            {
                nav_act = true;
            }
            else
            {
                nav_act = false;
            }
            if (yawinst && !nav_act || pitchinst && !nav_act || rollinst && !nav_act || reset_light_actual.Enabled)
            {
                navinst = true;
                drone_status = 23;
            }
            else navinst = false;
            Vector3D rc_xyz = rc_actual.GetPosition();
            if (cstm_dat_rd == 1 && cmd_read_ack == 0)
            {
                cmd_read_ack = 1;
                clr_cords = false;
            }

            if (!clr_cords && cstm_dat_rd == 1)
            {
                clr_cords = true;
                add_nav_Waypoint_mn = false;
                add_mine_waypoint = false;
                rc_actual.ClearWaypoints();
                rc_actual.SetAutoPilotEnabled(false);
            }
            if (clr_cords && cstm_dat_rd == 1)
            {
                if (nav_state && cmd_chng && is_undocked)
                {
                    main_nav_sequence = 1;
                    drone_output_status = "Nav";
                }
                if (mine_state && cmd_chng && is_undocked)
                {
                    mining_stage = 0;
                    distance_id = false;
                }
            }
            if (!add_nav_Waypoint_mn && main_nav_sequence == 1 && cstm_dat_rd == 1 && nav_state && !connector_actual.IsConnected && is_undocked)
            {
                rc_actual.ClearWaypoints();
                add_nav_Waypoint_mn = true;
                main_nav_complete = false;
                main_nav_sequence = 2;
                GetCustomData();
                rc_actual.AddWaypoint(main_gps_coords, "mine nav gps");
                drone_status = 1;
                drone_output_status = "Nav";
                if (!undock_light_actual.Enabled)
                {
                    undock_light_actual.Enabled = true;
                }
                if (dock_light_actual.Enabled)
                {
                    dock_light_actual.Enabled = false;
                }
            }
            if (main_nav_sequence == 2 && rc_xyz != rc_actual.CurrentWaypoint.Coords && nav_state && is_undocked && !main_nav_complete && add_nav_Waypoint_mn)
            {
                main_nav_sequence = 3;
                rc_actual.SpeedLimit = nav_spd;

                if (command_request == 1)
                {
                    rc_actual.SetCollisionAvoidance(true);
                    rc_actual.SetDockingMode(false);
                    rc_actual.SetAutoPilotEnabled(!navinst);
                    drone_status = 1;
                    reset_ai();
                    reset_light_actual.Enabled = false;

                }
                if (command_request == 2)
                {
                    rc_actual.SetCollisionAvoidance(false);
                    rc_actual.SetDockingMode(false);
                    rc_actual.SetAutoPilotEnabled(!navinst);
                    drone_status = 2;
                    reset_ai();
                    reset_light_actual.Enabled = false;
                }
                if (command_request == 3)
                {
                    rc_actual.SetCollisionAvoidance(false);
                    rc_actual.SetDockingMode(true);
                    rc_actual.SetAutoPilotEnabled(!navinst);
                    drone_status = 3;
                }
                if (command_request == 4)
                {
                    rc_actual.SetCollisionAvoidance(true);
                    rc_actual.SetDockingMode(false);
                    rc_actual.SetAutoPilotEnabled(!navinst);
                    drone_status = 4;
                    if (Collision_sense_enabled)
                    {
                        sensor_actual.Enabled = true;
                    }
                }
                drone_output_status = "Nav";
                if (!undock_light_actual.Enabled)
                {
                    undock_light_actual.Enabled = true;
                }
                if (dock_light_actual.Enabled)
                {
                    dock_light_actual.Enabled = false;
                }
            }

            if (main_nav_sequence == 3 && navinst && command_request == 1 || main_nav_sequence == 3 && navinst && command_request == 4)
            {
                rc_actual.ClearWaypoints();
                main_nav_sequence = 1;
                add_nav_Waypoint_mn = false;
                reset_ai();
                reset_light_actual.Enabled = false;
            }
            double spd = rc_actual.GetShipSpeed();
            if (spd <= gsl && ns_count < nr_lim && main_nav_sequence == 3 && !reset_light_actual.Enabled && !navinst && command_request == 4 || spd <= gsl && ns_count < nr_lim && main_nav_sequence == 3 && !reset_light_actual.Enabled && !navinst && command_request == 1)
            {
                ns_count++;
            }
            if (main_nav_sequence == 3 && reset_light_actual.Enabled && command_request == 1 || main_nav_sequence == 3 && reset_light_actual.Enabled && command_request == 4 || r_delay)
            {
                rc_actual.ClearWaypoints();
                main_nav_sequence = 1;
                add_nav_Waypoint_mn = false;
                reset_ai();
                reset_light_actual.Enabled = false;
                r_delay = false;
                ns_count = 0;
            }

            double rc_cw_x = main_gps_coords.X;
            double rc_cw_y = main_gps_coords.Y;
            double rc_cw_z = main_gps_coords.Z;
            if (main_nav_sequence == 3 && rc_xyz != rc_actual.CurrentWaypoint.Coords && nav_state && is_undocked && !main_nav_complete && add_nav_Waypoint_mn && !rc_actual.IsAutoPilotEnabled && !navinst)
            {
                rc_actual.SpeedLimit = nav_spd;
                if (command_request == 1)
                {
                    rc_actual.SetCollisionAvoidance(true);
                    rc_actual.SetDockingMode(false);
                    rc_actual.SetAutoPilotEnabled(!navinst);
                    drone_status = 1;
                    reset_light_actual.Enabled = false;
                    reset_ai();
                }
                if (command_request == 2)
                {
                    rc_actual.SetCollisionAvoidance(false);
                    rc_actual.SetDockingMode(false);
                    rc_actual.SetAutoPilotEnabled(!navinst);
                    drone_status = 2;
                    reset_ai();
                    reset_light_actual.Enabled = false;
                }
                if (command_request == 3)
                {
                    rc_actual.SetCollisionAvoidance(false);
                    rc_actual.SetDockingMode(true);
                    rc_actual.SetAutoPilotEnabled(!navinst);
                    drone_status = 3;
                }
                if (command_request == 4)
                {
                    rc_actual.SetCollisionAvoidance(true);
                    rc_actual.SetDockingMode(true);
                    rc_actual.SetAutoPilotEnabled(!navinst);
                    drone_status = 4;
                    if (Collision_sense_enabled)
                    {
                        sensor_actual.Enabled = true;
                    }

                }
                drone_output_status = "Nav";
                if (!undock_light_actual.Enabled)
                {
                    undock_light_actual.Enabled = true;
                }
                if (dock_light_actual.Enabled)
                {
                    dock_light_actual.Enabled = false;
                }
            }
            if (main_nav_sequence == 3 && rc_xyz.X >= rc_cw_x - nav_prec && rc_xyz.X <= rc_cw_x + nav_prec && rc_xyz.Y >= rc_cw_y - nav_prec && rc_xyz.Y <= rc_cw_y + nav_prec && rc_xyz.Z >= rc_cw_z - nav_prec && rc_xyz.Z <= rc_cw_z + nav_prec && nav_state && is_undocked && !main_nav_complete && add_nav_Waypoint_mn)
            {
                main_nav_sequence = 4;
                main_nav_complete = true;
                add_nav_Waypoint_mn = false;
                rc_actual.SetCollisionAvoidance(true);
                rc_actual.SetDockingMode(false);
                rc_actual.SetAutoPilotEnabled(false);
                rc_actual.ClearWaypoints();
                drone_status = 5;
                drone_output_status = "Nav End";
                if (!undock_light_actual.Enabled)
                {
                    undock_light_actual.Enabled = true;
                }
                if (dock_light_actual.Enabled)
                {
                    dock_light_actual.Enabled = false;
                }
                main_nav_sequence = 0;
                cmd_rqt = "0";
            }
            rc_actual.GetWaypointInfo(waypoints);
            if (main_nav_sequence == 3 && add_nav_Waypoint_mn && waypoints.Count <= 0 && !main_nav_complete && is_undocked && nav_state)
            {
                main_nav_sequence = 4;
                main_nav_complete = true;
                add_nav_Waypoint_mn = false;
                rc_actual.SetCollisionAvoidance(true);
                rc_actual.SetDockingMode(false);
                rc_actual.SetAutoPilotEnabled(false);
                rc_actual.ClearWaypoints();
                drone_status = 5;
                drone_output_status = "Nav End";
                if (!undock_light_actual.Enabled)
                {
                    undock_light_actual.Enabled = true;
                }
                if (dock_light_actual.Enabled)
                {
                    dock_light_actual.Enabled = false;
                }
                main_nav_sequence = 0;
                cmd_rqt = "0";
            }

            if (main_nav_sequence > 0 && recharge_request || main_nav_sequence > 0 && force_request_dock)
            {
                main_nav_sequence = 0;
                main_nav_complete = true;
                add_nav_Waypoint_mn = false;
                rc_actual.SetCollisionAvoidance(true);
                rc_actual.SetDockingMode(false);
                rc_actual.SetAutoPilotEnabled(false);
                rc_actual.ClearWaypoints();
                exit_waypoint_set = false;
                exit_sequence_complete = false;
                drone_status = 21;
                if (was_mining)
                {
                    reset_mining = true;
                }
                request_exit = false;
                was_mining = false;
                target_depth_achived = false;
                if (!is_docking || !is_undocking)
                {
                    reset_ai();
                }
                docking_stage = 1;
                collision_avoid_light_actual.Enabled = true;
                if (Collision_sense_enabled)
                {
                    sensor_actual.Enabled = true;
                }
                if (!undock_light_actual.Enabled)
                {
                    undock_light_actual.Enabled = true;
                }
                if (dock_light_actual.Enabled)
                {
                    dock_light_actual.Enabled = false;
                }
                drone_output_status = "RTB";
            }
            // *** Mining sequence ***
            if (!distance_id && mine_state && cstm_dat_rd == 1 && mining_stage == 0 && !is_autopiloting && is_undocked)
            {
                mine_coords_adjusted = false;
                tgt_drill_start.X = main_gps_coords.X;
                tgt_drill_start.Z = main_gps_coords.Z;
                tgt_drill_start.Y = main_gps_coords.Y;
                distance_id = true;
                if (trgt_vld)
                {
                    directionb = Vector3D.Normalize(new Vector3D(-(main_gps_coords - crnt_tgt_align)));
                }
                if (!trgt_vld)
                {
                    directionb = Vector3D.Normalize(new Vector3D(gravity));
                }
                Vector3D targetpositiont = directionb * drill_sl;
                tgt_drill_end.Y = Math.Round(tgt_drill_start.Y + targetpositiont.Y, 2);
                tgt_drill_end.X = Math.Round(tgt_drill_start.X + targetpositiont.X, 2);
                tgt_drill_end.Z = Math.Round(tgt_drill_start.Z + targetpositiont.Z, 2);
                drone_status = 6;
                drone_output_status = "Calculating mineshaft";
            }
            if (!target_depth_achived && rc_xyz.X >= tgt_drill_end.X - trm_prec && rc_xyz.X <= tgt_drill_end.X + trm_prec && rc_xyz.Y >= tgt_drill_end.Y - trm_prec && rc_xyz.Y <= tgt_drill_end.Y + trm_prec && rc_xyz.Z >= tgt_drill_end.Z - trm_prec && rc_xyz.Z <= tgt_drill_end.Z + trm_prec && mine_state && distance_id && is_undocked)
            {
                target_depth_achived = true;
            }
            if (target_depth_achived && !tunnel_sequence_finished)
            {
                tunnel_sequence_finished = true;
            }
            if (is_full && mine_state && distance_id)
            {
                cargo_full_achieved = true;
            }
            if (is_low_charge && mine_state && distance_id && !recharge_request_battery || is_low_charge && connector_actual.IsConnected && !recharge_request_battery || is_low_charge && !connector_actual.IsConnected && !recharge_request_battery && main_nav_sequence > 0 && !mine_state && is_undocked)
            {
                recharge_request_battery = true;
            }
            if (is_low_tank && mine_state && distance_id && !recharge_request_tank && !ignore_Htank || is_low_tank && connector_actual.IsConnected && !recharge_request_tank && !ignore_Htank || is_low_tank && !connector_actual.IsConnected && !recharge_request_tank && !ignore_Htank && main_nav_sequence > 0 && !mine_state && is_undocked)
            {
                recharge_request_tank = true;
            }
            if (!target_depth_achived && mining_stage == 0 && mine_state && distance_id && !is_autopiloting && !connector_actual.IsConnected && is_undocked)
            {
                mining_stage = 1;
                drone_status = 7;
                drone_output_status = "Initiating mining";
                reset_ai();
                reset_light_actual.Enabled = false;
            }
            if (mining_stage == 1 && !target_depth_achived && mine_state && distance_id && !is_autopiloting && is_undocked) // scan coordinate position to ground
            {
                if (!mine_coords_adjusted)
                {
                    mining_stage = 2;
                    mine_coords_adjusted = true;
                    if (trgt_vld)
                    {
                        direction = Vector3D.Normalize(new Vector3D(-(main_gps_coords - crnt_tgt_align)));
                    }
                    if (!trgt_vld)
                    {
                        direction = Vector3D.Normalize(new Vector3D(gravity));
                    }
                    Vector3D targetposition = direction * req_dist;
                    mining_gps_coords_temp.X = Math.Round(rc_xyz.X + targetposition.X, 2);
                    mining_gps_coords_temp.Y = Math.Round(rc_xyz.Y + targetposition.Y, 2);
                    mining_gps_coords_temp.Z = Math.Round(rc_xyz.Z + targetposition.Z, 2);
                    StDrlOnOff(true, cnvyrsON);
                    drone_status = 8;
                    drone_output_status = "Mining";
                }
            }
            if (mining_stage == 2 && !add_mine_waypoint && !target_depth_achived && mine_state && distance_id && !is_autopiloting && is_undocked)
            {
                mining_stage = 3;
                add_mine_waypoint = true;
                mining_gps_coords.X = mining_gps_coords_temp.X;
                mining_gps_coords.Y = mining_gps_coords_temp.Y;
                mining_gps_coords.Z = mining_gps_coords_temp.Z;
                rc_actual.AddWaypoint(mining_gps_coords, "mineloc");
                drone_status = 9;
                drone_output_status = "Mining+";
            }
            if (mining_stage == 3 && add_mine_waypoint && !target_depth_achived && mine_state && distance_id && !is_autopiloting && is_undocked)
            {
                mining_stage = 4;
                rc_actual.SpeedLimit = drill_spd;
                rc_actual.SetCollisionAvoidance(false);
                rc_actual.SetDockingMode(true);
                rc_actual.SetAutoPilotEnabled(!navinst);
                drone_status = 10;
                drone_output_status = "Mining++";
            }
            if (mining_stage == 3 && add_mine_waypoint && !target_depth_achived && mine_state && distance_id && !is_autopiloting && is_undocked && !rc_actual.IsAutoPilotEnabled)
            {
                mining_stage = 2;
                add_mine_waypoint = false;
                drone_status = 10;
                drone_output_status = "Mining++";
            }
            double rc_cmw_x = mining_gps_coords.X;
            double rc_cmw_y = mining_gps_coords.Y;
            double rc_cmw_z = mining_gps_coords.Z;
            if (mining_stage == 4 && !mining_nav_complete && add_mine_waypoint && !target_depth_achived && rc_actual.CurrentWaypoint.Name == null && mine_state && distance_id && !request_exit && !is_autopiloting && is_undocked)
            {
                mining_stage = 1;
                rc_actual.SetAutoPilotEnabled(!navinst);
                add_mine_waypoint = false;
                mine_coords_adjusted = false;
                drone_status = 11;
                drone_output_status = "Mining++-";
            }
            if (mining_stage == 4 && !mining_nav_complete && add_mine_waypoint && !target_depth_achived && !rc_actual.IsAutoPilotEnabled && mine_state && distance_id && !request_exit && !is_autopiloting && is_undocked)
            {
                mining_stage = 2;
                rc_actual.SetAutoPilotEnabled(!navinst);
                add_mine_waypoint = false;
                drone_status = 11;
                drone_output_status = "Mining+++";
            }

            if (mining_stage == 4 && !mining_nav_complete && add_mine_waypoint && !target_depth_achived && rc_xyz.X >= rc_cmw_x - mine_prec && rc_xyz.X <= rc_cmw_x + mine_prec && rc_xyz.Y >= rc_cmw_y - mine_prec && rc_xyz.Y <= rc_cmw_y + mine_prec && rc_xyz.Z >= rc_cmw_z - mine_prec && rc_xyz.Z <= rc_cmw_z + mine_prec && mine_state && distance_id && !request_exit && !is_autopiloting && is_undocked)
            {
                mining_stage = 5;
                mining_nav_complete = true;
                rc_actual.SetCollisionAvoidance(false);
                rc_actual.SetDockingMode(true);
                rc_actual.SetAutoPilotEnabled(false);
                drone_status = 12;
                drone_output_status = "Mining++++";
            }

            if (mining_stage == 4 && mining_nav_complete && add_mine_waypoint && !target_depth_achived && !rc_actual.IsAutoPilotEnabled && mine_state && distance_id && !request_exit && !is_autopiloting && is_undocked)
            {
                mining_stage = 5;
                rc_actual.SetCollisionAvoidance(false);
                rc_actual.SetDockingMode(true);
                rc_actual.SetAutoPilotEnabled(false);
                mining_nav_complete = true;
                drone_status = 12;
                drone_output_status = "Mining+++";
            }
            if (mining_stage == 4 && !mining_nav_complete && !target_depth_achived && dat_invalid && was_mining && distance_id && !request_exit && !is_autopiloting && is_undocked)
            {
                mining_stage = 6;
                request_exit = true;
                Last_Coords_Term = main_gps_coords;
                exit_waypoint_set = false;
                exit_sequence_complete = false;
                rc_actual.SpeedLimit = exit_spd;
                rc_actual.ClearWaypoints();
                rc_actual.SetCollisionAvoidance(false);
                rc_actual.SetDockingMode(false);
                rc_actual.SetAutoPilotEnabled(false);
                drone_status = 13;
                drone_output_status = "Terminating mining";
            }
            if (mining_stage >= 1 && mining_stage <= 4 && !mining_nav_complete && !target_depth_achived && request_exit && was_mining && distance_id && rc_actual.CurrentWaypoint.Name != "exit shaft" && !is_autopiloting && is_undocked)
            {
                mining_stage = 6;
                Last_Coords_Term = main_gps_coords;
                exit_waypoint_set = false;
                exit_sequence_complete = false;
                rc_actual.SpeedLimit = exit_spd;
                rc_actual.SetCollisionAvoidance(false);
                rc_actual.SetDockingMode(false);
                rc_actual.SetAutoPilotEnabled(false);
                rc_actual.ClearWaypoints();
                drone_status = 13;
                drone_output_status = "Terminating mining";
            }
            if (force_request_dock && mining_stage >= 1 && mining_stage <= 4 && !mining_nav_complete && dat_valid && cstm_dat_rd == 1 && was_mining && distance_id && !request_exit && !is_autopiloting && is_undocked)
            {

                mining_stage = 6;
                request_exit = true;
                Last_Coords_Term = main_gps_coords;
                exit_waypoint_set = false;
                exit_sequence_complete = false;
                rc_actual.SpeedLimit = exit_spd;
                rc_actual.ClearWaypoints();
                rc_actual.SetCollisionAvoidance(false);
                rc_actual.SetDockingMode(false);
                rc_actual.SetAutoPilotEnabled(false);
                drone_status = 14;
                drone_output_status = "Terminating mining";
            }
            if (mining_stage == 5 && mining_nav_complete && force_request_dock && mine_state && distance_id && !request_exit && !is_autopiloting && is_undocked)
            {
                mining_stage = 6;
                request_exit = true;
                Last_Coords_Term = main_gps_coords;
                rc_actual.SpeedLimit = exit_spd;
                rc_actual.SetCollisionAvoidance(false);
                rc_actual.SetDockingMode(false);
                rc_actual.SetAutoPilotEnabled(false);
                rc_actual.ClearWaypoints();
                exit_waypoint_set = false;
                exit_sequence_complete = false;
                drone_status = 16;
                drone_output_status = "Terminating mining";
            }
            if (mining_stage == 5 && mining_nav_complete && !target_depth_achived && !force_request_dock && mine_state && distance_id && !request_exit && !is_autopiloting && is_undocked)
            {
                mining_stage = 1;
                target_depth_achived = false;
                rc_actual.SpeedLimit = exit_spd;
                rc_actual.SetCollisionAvoidance(false);
                rc_actual.SetDockingMode(false);
                rc_actual.SetAutoPilotEnabled(false);
                rc_actual.ClearWaypoints();
                mine_coords_adjusted = false;
                add_mine_waypoint = false;
                mining_nav_complete = false;
                drone_status = 15;
                drone_output_status = "Mining";
            }
            distance_current = (rc_actual.GetPosition() - tgt_drill_end).Length();
            if (distance_current <= drill_sl - no_cnvy_dst || connector_actual.IsConnected || sens_convOPN)
            {
                cnvyrsON = true;
            }
            else
            {
                cnvyrsON = false;
            }
            if (mining_stage == 6 && !exit_waypoint_set && !exit_sequence_complete && was_mining && distance_id && request_exit && !is_autopiloting && is_undocked)
            {
                mining_stage = 7;
                exit_WPadj = false;
                exit_waypoint_set = true;
                exit_sequence_complete = false;
                reset_ai();
                reset_light_actual.Enabled = false;
                rc_actual.SpeedLimit = exit_spd;
                if (trgt_vld)
                {
                    directionc = Vector3D.Normalize(new Vector3D(-(main_gps_coords - crnt_tgt_align)));
                }
                if (!trgt_vld)
                {
                    directionc = Vector3D.Normalize(new Vector3D(gravity));
                }
                Vector3D targetpositione = directionc * drill_el;
                Vector3D targetpositione_temp = directionc * req_dist;
                tgt_drill_exit.Y = Math.Round(tgt_drill_start.Y - targetpositione.Y, 2);
                tgt_drill_exit.X = Math.Round(tgt_drill_start.X - targetpositione.X, 2);
                tgt_drill_exit.Z = Math.Round(tgt_drill_start.Z - targetpositione.Z, 2);
                drone_status = 17;
                drone_output_status = "Exit path";
            }
            if (mining_stage == 7 && exit_waypoint_set && !exit_sequence_complete && distance_id && request_exit && !is_autopiloting && is_undocked)
            {
                if (!exit_WPadj)
                {
                    mining_stage = 8;
                    exit_WPadj = true;
                    if (trgt_vld)
                    {
                        direction = Vector3D.Normalize(new Vector3D(-(main_gps_coords - crnt_tgt_align)));
                    }
                    if (!trgt_vld)
                    {
                        direction = Vector3D.Normalize(new Vector3D(gravity));
                    }
                    Vector3D targetposition = direction * req_dist;
                    exit_gps_coords_temp.X = Math.Round(rc_xyz.X - targetposition.X, 2);
                    exit_gps_coords_temp.Y = Math.Round(rc_xyz.Y - targetposition.Y, 2);
                    exit_gps_coords_temp.Z = Math.Round(rc_xyz.Z - targetposition.Z, 2);
                    rc_actual.AddWaypoint(exit_gps_coords_temp, "exit shaft");
                    drone_status = 18;
                    drone_output_status = "Exiting mineshaft";
                }
            }
            if (mining_stage == 8 && exit_WPadj && !exit_sequence_complete && was_mining && distance_id && request_exit && !is_autopiloting && is_undocked)
            {
                mining_stage = 9;
                rc_actual.SetCollisionAvoidance(false);
                rc_actual.SetDockingMode(false);
                rc_actual.SetAutoPilotEnabled(!navinst);
                drone_status = 18;
                drone_output_status = "Exiting mineshaft";
            }
            if (mining_stage == 9 && exit_waypoint_set && !exit_sequence_complete && was_mining && distance_id && request_exit && rc_xyz != tgt_drill_exit && rc_actual.CurrentWaypoint.Name != "exit shaft" && !is_autopiloting && is_undocked)
            {
                rc_actual.SetCollisionAvoidance(false);
                rc_actual.SetDockingMode(false);
                rc_actual.SetAutoPilotEnabled(!navinst);
                drone_status = 18;
                drone_output_status = "Exiting mineshaft";
                if (docking_stage > 0)
                {
                    drone_output_status = "Returning to dock";
                }
            }
            if (mining_stage == 9 && exit_waypoint_set && !exit_sequence_complete && was_mining && distance_id && request_exit && rc_xyz != tgt_drill_exit && !rc_actual.IsAutoPilotEnabled && !is_autopiloting && is_undocked)
            {
                rc_actual.SetCollisionAvoidance(false);
                rc_actual.SetDockingMode(false);
                rc_actual.SetAutoPilotEnabled(!navinst);
                drone_status = 18;
                drone_output_status = "Exiting mineshaft reloading WP";
                if (docking_stage > 0)
                {
                    drone_output_status = "Returning to dock";
                }
            }
            if (mining_stage == 9 && exit_waypoint_set && !exit_sequence_complete && was_mining && distance_id && !rc_actual.IsAutoPilotEnabled && !is_autopiloting && is_undocked && exit_WPadj)
            {
                mining_stage = 7;
                exit_WPadj = false;
                drone_output_status = "Exiting mineshaft reloading WP 2";
                if (docking_stage > 0)
                {
                    drone_output_status = "Returning to dock";
                }
            }
            double rc_cew_x = tgt_drill_exit.X;
            double rc_cew_y = tgt_drill_exit.Y;
            double rc_cew_z = tgt_drill_exit.Z;

            if (mining_stage == 9 && !exit_sequence_complete && exit_waypoint_set && rc_xyz.X >= tgt_drill_exit.X - nav_prec2 && rc_xyz.X <= tgt_drill_exit.X + nav_prec2 && rc_xyz.Y >= tgt_drill_exit.Y - nav_prec2 && rc_xyz.Y <= tgt_drill_exit.Y + nav_prec2 && rc_xyz.Z >= tgt_drill_exit.Z - nav_prec2 && rc_xyz.Z <= tgt_drill_exit.Z + nav_prec2 && distance_id && was_mining && request_exit && !is_autopiloting && is_undocked)
            {
                mining_stage = 10;
                exit_sequence_complete = true;
                exit_waypoint_set = true;
                rc_actual.SetCollisionAvoidance(false);
                rc_actual.SetDockingMode(false);
                rc_actual.SetAutoPilotEnabled(false);
                drone_status = 19;
                drone_output_status = "Exit Clear";
            }
            if (mining_stage == 9 && !exit_sequence_complete && exit_waypoint_set && distance_current >= (drill_sl + drill_el) && distance_id && was_mining && request_exit && !is_autopiloting && is_undocked)
            {
                mining_stage = 10;
                exit_sequence_complete = true;
                exit_waypoint_set = true;
                rc_actual.SetCollisionAvoidance(false);
                rc_actual.SetDockingMode(false);
                rc_actual.SetAutoPilotEnabled(false);
                drone_status = 19;
                drone_output_status = "Exit Clear";
            }
            if (mining_stage == 9 && !exit_sequence_complete && exit_waypoint_set && rc_xyz.X >= exit_gps_coords_temp.X - nav_prec && rc_xyz.X <= exit_gps_coords_temp.X + nav_prec && rc_xyz.Y >= exit_gps_coords_temp.Y - nav_prec && rc_xyz.Y <= exit_gps_coords_temp.Y + nav_prec && rc_xyz.Z >= exit_gps_coords_temp.Z - nav_prec && rc_xyz.Z <= exit_gps_coords_temp.Z + nav_prec && distance_id && was_mining && request_exit && !is_autopiloting && is_undocked)
            {
                mining_stage = 7;
                exit_WPadj = false;
                rc_actual.SetCollisionAvoidance(false);
                rc_actual.SetDockingMode(false);
                rc_actual.SetAutoPilotEnabled(false);
                drone_status = 19;
                drone_output_status = "Getting next WP";
            }
            if (mining_stage == 9 && was_mining && rc_actual.CurrentWaypoint.Name != "exit shaft" && request_exit && distance_id && !exit_waypoint_set && !exit_sequence_complete && stop_state && !is_autopiloting && is_undocked)
            {
                rc_actual.AddWaypoint(exit_gps_coords_temp, "exit shaft");
                rc_actual.SetCollisionAvoidance(false);
                rc_actual.SetDockingMode(false);
                rc_actual.SetAutoPilotEnabled(!navinst);
                drone_output_status = "Exiting mineshaft";

            }
            if (mining_stage == 10 && exit_waypoint_set && exit_sequence_complete && distance_id && request_exit && !is_autopiloting && is_undocked)
            {
                mining_stage = 11;
                rc_actual.ClearWaypoints();
                exit_waypoint_set = false;
                exit_sequence_complete = false;
                request_exit = false;
                drone_status = 20;
                drone_output_status = "Exit Clear";
            }
            if (mining_stage == 11 && target_depth_achived && was_mining && distance_id && !request_exit && !is_autopiloting && is_undocked)
            {
                rc_actual.ClearWaypoints();
                exit_waypoint_set = false;
                exit_sequence_complete = false;

                drone_status = 21;

                if (was_mining)
                {
                    reset_mining = true;
                }
                request_exit = false;
                was_mining = false;
                target_depth_achived = false;
                if (!is_docking || !is_undocking)
                {
                    reset_ai();
                }
                docking_stage = 1;
                collision_avoid_light_actual.Enabled = true;
                if (Collision_sense_enabled)
                {
                    sensor_actual.Enabled = true;
                }
                if (!undock_light_actual.Enabled)
                {
                    undock_light_actual.Enabled = true;
                }
                if (dock_light_actual.Enabled)
                {
                    dock_light_actual.Enabled = false;
                }
                drone_output_status = "RTB";
            }
            if (mining_stage == 11 && !target_depth_achived && was_mining && distance_id && !request_exit && !is_autopiloting && is_undocked)
            {
                rc_actual.ClearWaypoints();
                exit_waypoint_set = false;
                exit_sequence_complete = false;
                drone_status = 22;
                if (was_mining)
                {
                    reset_mining = true;
                }
                request_exit = false;
                if (!is_docking || !is_undocking)
                {
                    reset_ai();
                }
                drone_output_status = "Returning to dock";
                main_nav_sequence = 0;
                docking_stage = 1;
                collision_avoid_light_actual.Enabled = true;
                precM_light_actual.Enabled = false;
                if (Collision_sense_enabled)
                {
                    sensor_actual.Enabled = true;
                }
                if (!undock_light_actual.Enabled)
                {
                    undock_light_actual.Enabled = true;
                }
                if (dock_light_actual.Enabled)
                {
                    dock_light_actual.Enabled = false;
                }
            }
            if (reset_light_actual.Enabled && docking_stage > 0)
            {
                reset_ai();
                reset_light_actual.Enabled = false;
                docking_stage = 1;
                drone_output_status = "Reset Docking Sequence";
                if (!undock_light_actual.Enabled)
                {
                    undock_light_actual.Enabled = true;
                }
                if (dock_light_actual.Enabled)
                {
                    dock_light_actual.Enabled = false;
                }
            }
            if (docking_stage > 0 && precM_light_actual.Enabled)
            {
                if (Collision_sense_enabled)
                {
                    sensor_actual.Enabled = false;
                }
                collision_avoid_light_actual.Enabled = false;
                ai_move_actual.PrecisionMode = true;
                ai_move_actual.CollisionAvoidance = false;
            }
            if (docking_stage > 0 && !precM_light_actual.Enabled)
            {
                ai_move_actual.PrecisionMode = false;
            }
            if (docking_stage > 0 && collision_avoid_light_actual.Enabled)
            {
                ai_move_actual.CollisionAvoidance = true;
            }

            if (docking_stage == 1)
            {

                connector_actual.Enabled = true;
                StDrlOnOff(false, cnvyrsON);
                string locked = connector_actual.Status.ToString();
                if (!locked.Equals(cc) && docking_stage == 1)
                {
                    if (!skip_prec_mode)
                    {
                        ai_move_actual.PrecisionMode = true;
                    }
                    ai_move_actual.CollisionAvoidance = true;
                    if (skip_prec_mode && ai_move_actual.CollisionAvoidance || !skip_prec_mode && ai_move_actual.PrecisionMode && ai_move_actual.CollisionAvoidance)
                    {
                        ai_move_actual.GetActionWithName(ab1).Apply(ai_move_actual);
                        ai_dck_act.GetActionWithName(ab1).Apply(ai_dck_act);
                        ai_dck_act.GetActionWithName(p1).Apply(ai_dck_act);
                        collision_avoid_light_actual.Enabled = true;
                        undock_light_actual.Enabled = false;
                        if (Collision_sense_enabled)
                        {
                            sensor_actual.Enabled = true;
                        }
                    }
                    docking_stage = 2;
                    drone_output_status = "Docking";
                }
                else
                {
                    docking_stage = 2;
                    drone_output_status = "Returning to dock";
                }
            }
            if (docking_stage == 2)
            {
                gspeed = rc_actual.GetShipSpeed();
                string locked = connector_actual.Status.ToString();
                if (!locked.Equals(cc) && !precM_light_actual.Enabled && sensor_actual.Enabled && !reset_light_actual.Enabled && gspeed < gsl && ns_c2 < nr_lim2 || !locked.Equals(cc) && precM_light_actual.Enabled && !sensor_actual.Enabled && !reset_light_actual.Enabled && gspeed < gsl && ns_c2 < nr_lim2 || !locked.Equals(cc) && precM_light_actual.Enabled && !sensor_actual.Enabled && !reset_light_actual.Enabled && gspeed < gsl && ns_c2 < nr_lim2 || !locked.Equals(cc) && !precM_light_actual.Enabled && sensor_actual.Enabled && !reset_light_actual.Enabled && gspeed < gsl && ns_c2 < nr_lim2)
                {
                    ns_c2++;
                }
                StDrlOnOff(false, cnvyrsON);
                if (locked.Equals(cc) && docking_stage == 2)
                {
                    docking_stage = 3;
                    connector_actual.Connect();
                    reset_mining = true;
                    drone_output_status = "Docked";
                    undocking_stage = 0;
                }
                if (!locked.Equals(cc) && !ai_dck_act.GetValue<bool>(p1) && docking_stage == 2 || nsr)
                {
                    reset_light_actual.Enabled = true;
                }


            }
            if (docking_stage == 3 && is_docked)
            {
                ns_c2 = 0;
                StDrlOnOff(false, cnvyrsON);
                if (!tb_TOFF_act.Enabled)
                {
                    tb_TOFF_act.Enabled = true;
                }
                tb_TOFF_act.Trigger();
                reset_ai();
                dock_light_actual.Enabled = true;
                undock_light_actual.Enabled = false;
                precM_light_actual.Enabled = false;
                if (recharge_request_battery)
                {
                    for (int i = 0; i < battery_tag.Count; i++)
                    {
                        battery_tag[i].ChargeMode = ChargeMode.Recharge;
                        drone_output_status = "Recharging";
                    }
                }
                if (!recharge_request_battery)
                {
                    for (int i = 0; i < battery_tag.Count; i++)
                    {
                        battery_tag[i].ChargeMode = ChargeMode.Auto;
                    }
                }
                if (recharge_request_tank && !ignore_Htank)
                {
                    for (int i = 0; i < hydrogen_tank_tag.Count; i++)
                    {
                        hydrogen_tank_tag[i].Stockpile = true;
                        drone_output_status = "Refilling";
                    }
                }
                if (!recharge_request_tank && !ignore_Htank)
                {
                    for (int i = 0; i < hydrogen_tank_tag.Count; i++)
                    {
                        hydrogen_tank_tag[i].Stockpile = false;
                    }
                }
                if (!recharge_request)
                {
                    docking_stage = 0;
                }
            }
            if (docking_stage >= 1 && docking_stage <= 2 && stop_state && is_docking && !is_docked)
            {
                reset_ai();
                docking_stage = 0;
            }
            if (connector_actual.IsConnected && ignore_Htank || connector_actual.IsConnected && !ignore_Htank)
            {
                for (int i = 0; i < hydrogen_tank_tag.Count; i++)
                {
                    hydrogen_tank_tag[i].Stockpile = true;
                }
            }
            if (!connector_actual.IsConnected && ignore_Htank)
            {
                for (int i = 0; i < hydrogen_tank_tag.Count; i++)
                {
                    hydrogen_tank_tag[i].Stockpile = false;
                }
            }
            if (connector_actual.IsConnected && cargo_full_achieved || connector_actual.IsConnected && !cargo_is_empty)
            {
                drone_output_status = "Docked Unloading";
                if (Collision_sense_enabled)
                {
                    sensor_actual.Enabled = false;
                }
                collision_avoid_light_actual.Enabled = false;
                reset_light_actual.Enabled = false;
            }
            if (connector_actual.IsConnected && recharge_request)
            {
                drone_output_status = "Docked Recharging";
                if (Collision_sense_enabled)
                {
                    sensor_actual.Enabled = false;
                }
                collision_avoid_light_actual.Enabled = false;
                reset_light_actual.Enabled = false;
            }
            if (connector_actual.IsConnected && !undock_state && !cargo_full_achieved && cargo_is_empty && !recharge_request)
            {
                drone_output_status = "Docked Idle";
                if (Collision_sense_enabled)
                {
                    sensor_actual.Enabled = false;
                }
                collision_avoid_light_actual.Enabled = false;
                reset_light_actual.Enabled = false;
            }
            if (!connector_actual.IsConnected)
            {
                if (dock_light_actual.Enabled)
                {
                    dock_light_actual.Enabled = false;
                }
            }
            if (connector_actual.IsConnected)
            {
                if (!dock_light_actual.Enabled)
                {
                    dock_light_actual.Enabled = true;

                }
                if (undock_light_actual.Enabled)
                {
                    undock_light_actual.Enabled = false;
                }
            }
            sb.Clear();
            sb.Append(D_I_N);
            sb.Append(":");
            sb.Append(drone_damage_status);
            sb.Append(":");
            sb.Append(tunnel_sequence_finished);
            sb.Append(":");
            sb.Append(drone_output_status);
            sb.Append(":");
            sb.Append(is_docked);
            sb.Append(":");
            sb.Append(is_undocked);
            sb.Append(":");
            sb.Append(is_autopiloting);
            sb.Append(":");
            sb.Append(rc_actual.IsAutoPilotEnabled);
            sb.Append(":");
            sb.Append(Math.Round(rc_xyz.X, 2));
            sb.Append(":");
            sb.Append(Math.Round(rc_xyz.Y, 2));
            sb.Append(":");
            sb.Append(Math.Round(rc_xyz.Z, 2));
            sb.Append(":");
            sb.Append(drill_sl);
            sb.Append(":");
            sb.Append(Math.Round(distance_current, 2));
            sb.Append(":");
            sb.Append(Math.Round((drill_sl - no_cnvy_dst), 2));
            sb.Append(":");
            sb.Append(Math.Round(percent_battery_power, 2));
            sb.Append(":");
            sb.Append(Math.Round(pcnt_gas_tank, 2));
            sb.Append(":");
            sb.Append(Math.Round(total_percent_cargo_used, 2));
            sb.Append(":");
            sb.Append(gpsindx);
            dat_out = sb.ToString();
            if (t_delay && pinged)
            {
                t_delay = false;
                t_count = 0;
            }

            if (pinged)
            {
                IGC.SendBroadcastMessage(tx_ch, dat_out);
                pinged = false;
                dat_in3 = "";
            }
            if (r_delay)
            {
                r_delay = false;
                ns_count = 0;
            }
            if (nsr)
            {
                nsr = false;
                ns_c2 = 0;
            }
            GetDroneStatus(drone_status);
            Echo("Drone ID: " + D_I_N + " # " + drone_damage_status);
            Echo("Status Ints: " + drnst);
            Echo("Drone Status: " + drone_output_status);
            Echo("Distance ID: " + distance_id);
            Echo("Command seq: " + cmd_rqt);
            Echo("Cargo: " + Math.Round(total_percent_cargo_used, 2) + "%  Full: " + cargo_full_achieved);
            Echo("Charge: " + Math.Round(percent_battery_power, 2) + "%  Recharge: " + recharge_request_battery);
            if (hydrogen_tank_tag.Count > 0)
            {
                Echo("HTank: " + Math.Round(pcnt_gas_tank, 2) + "%  Recharge: " + recharge_request_tank);
            }
            Echo("Mine distance: " + Math.Round(distance_current, 2) + "m" + " Mine Start: " + (drill_sl - no_cnvy_dst) + "m");
            Echo("Mine: " + mine_state + " - Stage:  " + mining_stage);
            Echo("Nav: " + nav_state + " - Stage:  " + main_nav_sequence);
            Echo("Dock: " + is_docked + " - Stage:  " + docking_stage);
            Echo("Undock: " + is_undocked + " - Stage  " + undocking_stage);
            Echo("Connected: " + connector_actual.IsConnected);
            Echo("Depth Achieved: " + target_depth_achived);
            Echo("Stopped: " + stop_state);
            Echo($"{t_count} {t_delay} {ns_count} {r_delay} {ns_c2} {nsr} {Math.Round(spd, 2)} {Math.Round(gspeed, 2)} {pinged}");
            t_count++;
            if (t_count >= t_lim)
            {
                t_delay = true;
            }
            if (ns_count >= nr_lim)
            {
                r_delay = true;
            }
            if (ns_c2 >= nr_lim2)
            {
                nsr = true;
            }

        }
        Vector3D GetNavAngles(Vector3D Target)
        {
            Vector3D RCcenter = rc_actual.GetPosition();
            Vector3D RCfow = rc_actual.WorldMatrix.Forward;
            Vector3D RCup = rc_actual.WorldMatrix.Up;
            Vector3D RCleft = rc_actual.WorldMatrix.Left;
            Vector3D RCright = rc_actual.WorldMatrix.Right;
            if (trgt_vld)
            {
                TrgtPitch = Math.Acos(Vector3D.Dot(RCfow, Vector3D.Reject(Vector3D.Normalize(Target - RCcenter), RCleft))) - (Math.PI / 2);
                TrgtRoll = Math.Acos(Vector3D.Dot(RCleft, Vector3D.Reject(Vector3D.Normalize(-(Target - RCcenter)), RCfow))) - (Math.PI / 2);

            }
            if (!trgt_vld)
            {

                TrgtPitch = Math.Acos(Vector3D.Dot(RCfow, Vector3D.Reject(Vector3D.Normalize(rc_actual.GetNaturalGravity()), RCleft))) - (Math.PI / 2);
                TrgtRoll = Math.Acos(Vector3D.Dot(RCleft, Vector3D.Reject(Vector3D.Normalize(-rc_actual.GetNaturalGravity()), RCfow))) - (Math.PI / 2);
                TrgtYaw = TrgtPitch;
            }
            return new Vector3D(TrgtYaw, -TrgtRoll, -TrgtPitch);

        }

        void SetGyroOverride(bool OverrideOnOff, Vector3 settings, float Power = 1)
        {

            if (gyro_tag.Count > 0)
            {
                for (int j = 0; j < gyro_tag.Count; j++)
                {
                    if (gyro_tag[j] == null)
                    {
                        setup_complete = false;
                    }
                    if (gyro_tag[j] != null)
                    {
                        gyro_actual = gyro_tag[j];
                        gyro_monitor = gyro_actual;
                        if (gyro_actual != null)
                        {
                            if ((!gyro_actual.GyroOverride && OverrideOnOff) || (gyro_actual.GyroOverride && !OverrideOnOff))
                                gyro_actual.ApplyAction("Override");
                            gyro_actual.SetValue("Power", Power);
                            gyro_actual.SetValue("Yaw", settings.GetDim(0));
                            gyro_actual.SetValue("Pitch", settings.GetDim(1));
                            gyro_actual.SetValue("Roll", settings.GetDim(2));
                        }
                    }
                }
            }
        }

        void GetCustomData()
        {
            String[] gps_cd = Me.CustomData.Split(':');
            if (gps_cd.Length > 5)
            {
                gpsindx = gps_cd[1];
                main_gps_coords = new Vector3D(Double.Parse(gps_cd[2]), Double.Parse(gps_cd[3]), Double.Parse(gps_cd[4]));
                cmd_rqt = gps_cd[6].ToString();
                if (int.TryParse(cmd_rqt, out command_request))
                {
                    int.TryParse(cmd_rqt, out command_request);
                }
                else
                {
                    command_request = 0;
                }
                cmd_dist = gps_cd[7].ToString();
                if (Double.TryParse(cmd_dist, out drill_sl))
                {
                    Double.TryParse(cmd_dist, out drill_sl);
                }
                else
                {
                    drill_sl = 1.0;
                }
            }


            if (gps_cd.Length < 9)
            {

                no_cnvy_dst = 0.0;
                return;
            }

            if (gps_cd.Length > 9)
            {
                if (gps_cd[8] == null || gps_cd[8] == "")
                {
                    gps_dat_7 = "";
                    no_cnvy_dst = 0.0;
                }
                else
                {
                    gps_dat_7 = gps_cd[8].ToString();
                    if (double.TryParse(gps_dat_7, out no_cnvy_dst))
                    {
                        double.TryParse(gps_dat_7, out no_cnvy_dst);
                    }
                    else
                    {
                        no_cnvy_dst = 0.0;
                    }
                }
            }

            if (gps_cd.Length > 10)
            {
                if (gps_cd[9] == null || gps_cd[9] == "")
                {
                    gps_dat_8 = "";
                }
                else
                {
                    gps_dat_8 = gps_cd[9].ToString();
                }
            }

            if (gps_cd.Length > 11)
            {
                if (gps_cd[10] == null || gps_cd[10] == "")
                {
                    gps_dat_9 = "";
                }
                else
                {
                    gps_dat_9 = gps_cd[10].ToString();
                }
            }

            if (gps_cd.Length > 12)
            {
                if (gps_cd[11] == null || gps_cd[11] == "")
                {
                    gps_dat_10 = "";
                    dt_prsnt4 = false;
                }
                else
                {
                    gps_dat_10 = gps_cd[11].ToString();

                    if (double.TryParse(gps_dat_10, out tgtX))
                    {
                        double.TryParse(gps_dat_10, out tgtX);
                        dt_prsnt4 = true;
                    }
                    else
                    {
                        tgtX = 0.0;
                        dt_prsnt4 = false;
                    }
                }
            }

            if (gps_cd.Length > 13)
            {
                if (gps_cd[12] == null || gps_cd[12] == "")
                {
                    gps_dat_11 = "";
                    dt_prsnt5 = false;
                }
                else
                {
                    gps_dat_11 = gps_cd[12].ToString();

                    if (double.TryParse(gps_dat_11, out tgtY))
                    {
                        double.TryParse(gps_dat_11, out tgtY);
                        dt_prsnt5 = true;
                    }
                    else
                    {
                        tgtY = 0.0;
                        dt_prsnt5 = false;
                    }
                }
            }

            if (gps_cd.Length > 14)
            {
                if (gps_cd[13] == null || gps_cd[13] == "")
                {
                    gps_dat_12 = "";
                    dt_prsnt6 = false;
                }
                else
                {
                    gps_dat_12 = gps_cd[13].ToString();

                    if (double.TryParse(gps_dat_12, out tgtZ))
                    {
                        double.TryParse(gps_dat_12, out tgtZ);
                        dt_prsnt6 = true;
                    }
                    else
                    {
                        tgtZ = 0.0;
                        dt_prsnt6 = false;
                    }
                }
            }

            if (dt_prsnt4 && dt_prsnt5 && dt_prsnt6)
            {
                trgt_vld = true;
                align_tgt_new.X = tgtX;
                align_tgt_new.Y = tgtY;
                align_tgt_new.Z = tgtZ;
            }
            else
            {
                trgt_vld = false;
            }

            if (gps_cd.Length < 14)
            {
                dt_prsnt6 = false;
            }
            if (gps_cd.Length < 13)
            {
                dt_prsnt5 = false;
            }
            if (gps_cd.Length < 12)
            {
                dt_prsnt4 = false;
            }
        }

        void StDrlOnOff(bool DrilOnOf, bool UConv)
        {

            if (drill_tag.Count <= 0)
            {
                getnewdrills();
            }
            if (drill_tag.Count > 0)
            {
                for (int i = 0; i < drill_tag.Count; i++)
                {
                    if (drill_tag[i] == null)
                    {
                        setup_complete = false;
                    }
                    if (drill_tag[i] != null)
                    {
                        drl_act = drill_tag[i];
                    }
                    if (drl_act != null)
                    {
                        if (DrilOnOf && !drl_act.Enabled)
                        {
                            drl_act.Enabled = true;
                        }
                        if (!DrilOnOf && drl_act.Enabled)
                        {
                            drl_act.Enabled = false;
                        }
                        if (UConv)
                        {
                            drl_act.UseConveyorSystem = true;
                        }
                        else
                        {
                            drl_act.UseConveyorSystem = false;
                        }
                    }
                }
            }
        }
        void getnewdrills()
        {
            drill_all = new List<IMyShipDrill>();
            drill_tag = new List<IMyShipDrill>();
            GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(drill_all, b => b.CubeGrid == Me.CubeGrid);
            if (drill_all.Count > 0)
            {
                for (int i = 0; i < drill_all.Count; i++)
                {
                    if (drill_all[i].CustomName.Contains(D_I_N))
                    {
                        drill_tag.Add(drill_all[i]);
                    }
                    if (!drill_all[i].CustomName.Contains(D_I_N))
                    {
                        n = drill_all[i].CustomName;
                        drill_all[i].CustomName = n + " " + D_I_N;
                        drill_tag.Add(drill_all[i]);
                    }
                }
            }
        }

        void reset_ai()
        {
            ai_dck_act.GetActionWithName(ab0).Apply(ai_dck_act);
            ai_task_undock_actual.GetActionWithName(ab0).Apply(ai_task_undock_actual);
            ai_move_actual.GetActionWithName(ab0).Apply(ai_move_actual);
            collision_avoid_light_actual.Enabled = false;
            precM_light_actual.Enabled = false;
            if (Collision_sense_enabled)
            {
                sensor_actual.Enabled = false;
            }

        }
        void LoadStorageData()
        {
            if (_ini.TryParse(Storage))
            {
                var str = "";
                str = _ini.Get("commands", "c1").ToString();
                bool.TryParse(str, out recall);
                str = _ini.Get("commands", "c2").ToString();
                bool.TryParse(str, out stop_state);
                str = _ini.Get("commands", "c3").ToString();
                bool.TryParse(str, out was_mining);
                str = _ini.Get("commands", "c4").ToString();
                bool.TryParse(str, out nav_state);
                str = _ini.Get("commands", "c5").ToString();
                bool.TryParse(str, out mine_state);
                str = _ini.Get("commands", "c6").ToString();
                bool.TryParse(str, out dock_state);
                str = _ini.Get("commands", "c7").ToString();
                bool.TryParse(str, out mode_set);

                str = _ini.Get("dockmode", "d1").ToString();
                int.TryParse(str, out docking_stage);
                str = _ini.Get("dockmode", "d2").ToString();
                bool.TryParse(str, out undock_state);
                str = _ini.Get("dockmode", "d3").ToString();
                int.TryParse(str, out undocking_start);
                str = _ini.Get("dockmode", "d4").ToString();
                int.TryParse(str, out undocking_stage);

                str = _ini.Get("unitstate", "u1").ToString();
                bool.TryParse(str, out recharge_request);
                str = _ini.Get("unitstate", "u2").ToString();
                bool.TryParse(str, out nav_act);
                str = _ini.Get("unitstate", "u3").ToString();
                int.TryParse(str, out main_nav_sequence);
                str = _ini.Get("unitstate", "u4").ToString();
                bool.TryParse(str, out main_nav_complete);
                str = _ini.Get("unitstate", "u5").ToString();
                bool.TryParse(str, out add_nav_Waypoint_mn);
                str = _ini.Get("unitstate", "u6").ToString();
                bool.TryParse(str, out distance_id);
                str = _ini.Get("unitstate", "u7").ToString();
                int.TryParse(str, out mining_stage);
                str = _ini.Get("unitstate", "u8").ToString();
                bool.TryParse(str, out add_mine_waypoint);
                str = _ini.Get("unitstate", "u9").ToString();
                bool.TryParse(str, out mine_coords_adjusted);
                str = _ini.Get("unitstate", "u10").ToString();
                bool.TryParse(str, out target_depth_achived);
                str = _ini.Get("unitstate", "u11").ToString();
                bool.TryParse(str, out reset_mining);
                str = _ini.Get("unitstate", "u12").ToString();
                bool.TryParse(str, out mining_nav_complete);
                str = _ini.Get("unitstate", "u12").ToString();
                bool.TryParse(str, out force_request_dock);
                str = _ini.Get("unitstate", "u13").ToString();
                bool.TryParse(str, out request_exit);
                str = _ini.Get("unitstate", "u14").ToString();
                bool.TryParse(str, out exit_sequence_complete);
                str = _ini.Get("uunitstate16", "u15").ToString();
                bool.TryParse(str, out exit_waypoint_set);
                str = _ini.Get("unitstate", "u16").ToString();
                bool.TryParse(str, out tunnel_sequence_finished);
                str = _ini.Get("unitstate", "u18").ToString();
                bool.TryParse(str, out yawinst);
                str = _ini.Get("unitstate", "u19").ToString();
                bool.TryParse(str, out pitchinst);
                str = _ini.Get("unitstate", "u20").ToString();
                bool.TryParse(str, out rollinst);
                str = _ini.Get("unitstate", "u21").ToString();
                bool.TryParse(str, out navinst);
                str = _ini.Get("unitstate", "u22").ToString();
                double.TryParse(str, out distance_current);
                str = _ini.Get("coordinates", "c1").ToString();
                Vector3D.TryParse(str, out mining_gps_coords);
                str = _ini.Get("coordinates", "c2").ToString();
                Vector3D.TryParse(str, out mining_gps_coords_temp);
                str = _ini.Get("coordinates", "c3").ToString();
                Vector3D.TryParse(str, out tgt_drill_start);
                str = _ini.Get("coordinates", "c4").ToString();
                Vector3D.TryParse(str, out tgt_drill_end);
                str = _ini.Get("coordinates", "c5").ToString();
                Vector3D.TryParse(str, out tgt_drill_exit);
                str = _ini.Get("coordinates", "c6").ToString();
                Vector3D.TryParse(str, out exit_gps_coords_temp);
                str = _ini.Get("coordinates", "c7").ToString();
                Vector3D.TryParse(str, out main_gps_coords);
                str = _ini.Get("coordinates", "c8").ToString();
                Vector3D.TryParse(str, out crnt_tgt_align);
                str = _ini.Get("coordinates", "c9").ToString();
                Vector3D.TryParse(str, out align_tgt_new);
                str = _ini.Get("coordinates", "c10").ToString();
                Vector3D.TryParse(str, out directionb);
                str = _ini.Get("coordinates", "c11").ToString();
                Vector3D.TryParse(str, out direction);
                str = _ini.Get("coordinates", "c12").ToString();
                Vector3D.TryParse(str, out directionc);
                str = _ini.Get("coordinates", "c13").ToString();
                Vector3D.TryParse(str, out gravity);
                str = _ini.Get("coordinates", "co14").ToString();
                gpsindx = str;
            }
        
        }
            void GetDroneStatus(int drnstus)
        {

            if (drnstus == 0)
            {
                drnst = "Idle";
            }
            if (drnstus == 1 || drnstus == 4)
            {
                drnst = $"Nav CA {Collision_sense_enabled}";
            }
            if (drnstus == 2 || drnstus == 3)
            {
                drnst = "Nav P";
            }
            if (drnstus == 5)
            {
                drnst = "Navi Dest Reach";
            }
            if (drnstus == 6)
            {
                drnst = "Mine Calc shaft";
            }
            if (drnstus == 7)
            {
                drnst = "Mine Start";
            }
            if (drnstus == 8)
            {
                drnst = "Mine Calc WP";
            }
            if (drnstus == 9)
            {
                drnst = "Mine Add WP";
            }
            if (drnstus == 10)
            {
                drnst = "Mine to WP";
            }
            if (drnstus == 11)
            {
                drnst = "Mine En AP";
            }
            if (drnstus == 12)
            {
                drnst = "Mine WP reach";
            }
            if (drnstus == 13)
            {
                drnst = "Mine Trunc";
            }
            if (drnstus == 14)
            {
                drnst = "Mine Fin";
            }
            if (drnstus == 15)
            {
                drnst = "Mine new WP";
            }
            if (drnstus == 16)
            {
                drnst = "Mine Fnshd";
            }
            if (drnstus == 17)
            {
                drnst = "WP mine exit";
            }
            if (drnstus == 18)
            {
                drnst = "Nav mine exit";
            }
            if (drnstus == 19)
            {
                drnst = "Mine exit reach";
            }
            if (drnstus == 20)
            {
                drnst = "Cl WP dock";
            }
            if (drnstus == 21)
            {
                drnst = "Rtn dock";
            }
            if (drnstus == 22)
            {
                drnst = "Rtn unload";
            }
            if (drnstus == 23)
            {
                drnst = "Stablz";
            }
            if (drnstus == 24)
            {
                drnst = "Read dt";
            }
            if (drnstus == 25)
            {
                drnst = "Comp cmd data";
            }
        }
        //end program

    }
}
