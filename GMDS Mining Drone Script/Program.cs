using EmptyKeys.UserInterface.Generated.EditFactionIconView_Bindings;
using ProtoBuf;
using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using Sandbox.Game.Screens.Terminal.Controls;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Interfaces.Terminal;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        // General Mining Drone Script v0.517B       
        // Adomus o7 o7 o7
        // 
        // 
        #region mdk preserve
        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            manageFirstLoad(Storage, Me.CustomData);
        }
        //rename these for drone
        int drone_id_num = 1;
        string droneTag = "SWRM_D";

        //ore detection
        bool cargoSenseEnabled = true;
        float cargoSenseLimit = 0.0f;
        //dmg detect
        bool damageReportingEnabled = true;
        //collision sense ranges
        bool collisionSenseEnabled = true;
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
        bool autoChargeMode = true;
        float bat_CHGhi = 100.0f;
        float bat_CHGlow = 30.0f;
        //drone nav settings
        float drill_speed = 1.0f;
        float nav_speed = 5.0f;
        float exit_speed = 1.0f;
        double nav_inst_thr = 0.05;
        double currentSpeedNotMovingThreshold = 0.1;
        //drone mining settings
        double drillSetLength = 100.0;
        double drill_el = 20.0;
        double req_dist = 1.0;
        double nav_prec = 0.5;
        double nav_prec2 = 1.2;
        double mine_prec = 0.5;
        double terrainclearoffset = 9.0;



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
        string thrusters = "Thrusters";
        string debug = "Debug";
        string display = "Info";
        string autodockCommand = "autodock";
        string collisionSenseCommand = "collision";
        string cargoSenseCommand = "cargo";
        string manualAssignCommand = "manual";
        string terrainClearCommand = "keepterrain";
        string exitDistanceCommand = "exitdistance";
        string exitSpeedCommand = "exitspeed";
        string navSpeedCommand = "navspeed";
        string drillSpeedCommand = "drillspeed";
        string terrainDistanceCommand = "terraincleardistance";


        #endregion

        string ver = "V0.628B";
        //drone transmission settings
        int transmit_time_limit = 5;

        #region Global Declarations
        //other variables
        int no_speed_navigation_delay_limit = 5;
        int no_speed_undock_delay_limit = 120;
        int no_speed_dock_delay_limit = 360;
        double game_tick_length = 16.666;
        string D_I_N = "";
        string D_C_N = "";
        string dockTaskName = "";
        string UndockModeTagName = "";
        string Thr_ON_n = "";
        string Thr_OFF_N = "";
        string ResetTagName = "";
        string CA_T_N = "";
        string PrecisionModeTagName = "";
        string H_T_N = "";
        string D_S_C = "";
        string S_N_T = "";
        string damageLightTag = "";
        string pingChannel = "";
        string thrustGroupTag = "";
        string pingChannelTag = "ping";
        string syncChannelTag = "sync";
        string droneDamageStatus = "OK";
        string droneStatusOutput = "Idle";
        string recall_command = "recall";
        string secondary_tag = "";
        double termnationPrecision = 0.0;
        double terminationCoefficient = 0.02;
        float GyrMlt = 2;
        string dat_in;
        string dat_in2 = "";
        string pingedMessageDataIn = "";
        string dat_in4 = "";
        bool pinged = false;
        int droneStatus = 0;
        int commandRequest = 0;
        int cmd_rqold = 0;
        bool mode_set = false;
        string drnst;
        string commandCommandDataRequested = "0";
        //string commandDataDistance = "10.0";
        double ignoreDistance = 0.0;
        double alignmentTargetX = 0.0;
        double alignmentTargetY = 0.0;
        double alignmentTargetZ = 0.0;
        //comms channel
        string rx_ch;
        string tx_ch = "";
        string rx_channel_recall = "";
        string rx_channel_recall_drone = "";
        string rx_channel_sync = "";
        //logic flags
        float total_percent_cargo_used = 0.0f;
        //float ttl_PWRs;
        float ttl_sPWR;
        //float ttl_PWRm;
        float ttl_mPWR;
        //float ttl_cPWR;
        //float ttl_PWRc;
        bool can_gyroOVR = false;
        bool targetAlignmentValid = false;
        bool cnvyrsON = false;
        bool exitWaypointAdjusted = false;
        //double TrgtPitch = 0.0;
        //double TrgtRoll = 0.0;
        // double TrgtYaw = 0.0;
        double ttl_GASs;
        double ttl_sGAS;
        double ttl_mGAS;
        double ttl_GASm;
        bool sens_convOPN = false;
        bool force_request_dock = false;
        bool requestExit = false;
        bool commandChanged = false;
        bool stopState = true;
        bool mineState = false;
        bool navState = false;
        bool dockState = false;
        bool undockState = false;
        bool cargoIsFull = false;
        bool cargoIsEmpty = false;
        bool cargoFullAchieved = false;
        bool is_full_charge = false;
        bool is_low_charge = false;
        bool recharge_request = false;
        bool wasMining = false;
        bool isDocking = false;
        bool isUndocking = false;
        bool isAutopiloting = false;
        bool isDocked = false;
        bool isUndocked = false;
        bool reset_mining = false;
        bool clr_cords = false;
        int custom_data_read = 0;
        bool dataInvalid = false;
        bool dataValid = false;
        bool tunnelSequenceFinished = false;
        int undocking_start = 0;
        bool is_full_tank = false;
        bool is_low_tank = false;
        bool recharge_request_tank = false;
        bool recharge_request_battery = false;
        int t_count = 0;
        int no_speed_count_navigation_reset_delay_count = 0;
        int no_speed_undock_delay_count = 0;
        int no_speed_dock_delay_count = 0;
        bool no_speed_ready_undock = false;
        bool no_speed_ready_dock = false;
        bool transmit_delay = false;
        bool recall = false;
        bool commandDataPresent_11 = false;
        bool commandDataPresent_12 = false;
        bool commandDataPresent_13 = false;
        string commandDataIgnoreDistance = "";
        //string commandData8 = "";
        //string commandData9 = "";
        //string commandDataAlignX = "";
        //string commandDataAlignY = "";
        //string commandDataAlignZ = "";
        string gpsIndex = "";
        int cmd_read_ack = 0;
        int mainNavSequence = 0;
        bool add_nav_Waypoint_mn = false;
        bool main_nav_complete = false;
        IMyRemoteControl remoteControlActual;
        //IMyCameraBlock camera_actual;
        IMyShipConnector connectorActual;
        IMyRadioAntenna antenna_actual;
        IMyTimerBlock timerBlockTONActual, timerBlockTOFFActual;
        IMyPathRecorderBlock ai_task_dock_actual, ai_task_undock_actual;
        IMyFlightMovementBlock ai_move_actual;
        IMyBatteryBlock crntbatteryblock;
        IMyLightingBlock dockLightActual, undockLightActual, collisionAvoidLightActual, precModeLightActual, resetLightActual, damageLightActual;
        IMySensorBlock sensorActual;
        IMyGasTank crnthyrdogentank;
        Vector3D main_gps_coords;
        Vector3D mining_gps_coords;
        Vector3D mining_gps_coords_temp;
        Vector3D tgt_drill_start;
        Vector3D tgt_drill_end;
        Vector3D tgt_drill_exit;
        Vector3D exit_gps_coords_temp;
        //Vector3D Last_Coords_Term;
        Vector3D crnt_tgt_align;
        Vector3D alignmentTargetNew;
        Vector3D directionb;
        Vector3D direction;
        Vector3D directionc;
        Vector3D gravity;
        bool navigation_reset_delay = false;
        bool miningInitialised = false;
        bool exitWaypointSet = false;
        bool exitSequenceComplete = false;
        bool mining_nav_complete = false;
        bool targetDepthAchieved = false;
        bool mine_coords_adjusted = false;
        bool add_mine_waypoint = false;
        int miningStage = 0;
        int dockingStage = 0;
        int undocking_stage = 0;
        bool yawinst = false;
        bool pitchinst = false;
        bool rollinst = false;
        bool navinst = false;
        double distance_current = 0;
        bool nav_act = false;
        string abr = "ActivateBehavior";
        string ab0 = "ActivateBehavior_Off";
        string ab1 = "ActivateBehavior_On";
        //string cc = "Connectable";
        string p1 = "ID_PLAY_CHECKBOX";
        string pon = "ID_PLAY_CHECKBOX_On";
        string poff = "ID_PLAY_CHECKBOX_Off";
        List<IMyRemoteControl> rc_all = new List<IMyRemoteControl>();
        List<IMyRemoteControl> rctag = new List<IMyRemoteControl>();
        List<IMySensorBlock> sensor_all = new List<IMySensorBlock>();
        List<IMySensorBlock> sensor_tag = new List<IMySensorBlock>();
        List<IMyCameraBlock> cam_all = new List<IMyCameraBlock>();
        List<IMyCameraBlock> camera_tag = new List<IMyCameraBlock>();
        List<IMyShipConnector> connector_all = new List<IMyShipConnector>();
        List<IMyShipConnector> connector_tag = new List<IMyShipConnector>();
       // List<IMyShipConnector> refresh_tag = new List<IMyShipConnector>();
        List<IMyCargoContainer> cargo_all = new List<IMyCargoContainer>();
        List<IMyCargoContainer> cargo_tag = new List<IMyCargoContainer>();
        List<IMyCargoContainer> cargo_sense = new List<IMyCargoContainer>();
        List<IMyRadioAntenna> antenna_all = new List<IMyRadioAntenna>();
        List<IMyRadioAntenna> antenna_tag = new List<IMyRadioAntenna>();
        List<IMyBeacon> beacons_all = new List<IMyBeacon>();
        List<IMyBeacon> beacons_tag = new List<IMyBeacon>();
        List<IMyPathRecorderBlock> flight_path_all = new List<IMyPathRecorderBlock>();
        List<IMyPathRecorderBlock> flight_path_dock_tag = new List<IMyPathRecorderBlock>();
        List<IMyPathRecorderBlock> flight_path_undock_tag = new List<IMyPathRecorderBlock>();
        List<IMyFlightMovementBlock> flight_move_all = new List<IMyFlightMovementBlock>();
        List<IMyFlightMovementBlock> flight_move_tag = new List<IMyFlightMovementBlock>();
        List<IMyTimerBlock> timer_block_all = new List<IMyTimerBlock>();
        List<IMyTimerBlock> timer_block_tON_tag = new List<IMyTimerBlock>();
        List<IMyTimerBlock> timer_block_tOFF_tag = new List<IMyTimerBlock>();
        List<IMyTimerBlock> timer_block_precM_tag = new List<IMyTimerBlock>();
        List<IMyTimerBlock> timer_block_undock_tag = new List<IMyTimerBlock>();
        List<IMyLightingBlock> light_all = new List<IMyLightingBlock>();
        List<IMyLightingBlock> lightUndockTag = new List<IMyLightingBlock>();
        List<IMyLightingBlock> light_dock_tag = new List<IMyLightingBlock>();
        List<IMyLightingBlock> light_collision_avoid_tag = new List<IMyLightingBlock>();
        List<IMyLightingBlock> lightPrecMTag = new List<IMyLightingBlock>();
        List<IMyLightingBlock> lightResetTag = new List<IMyLightingBlock>();
        List<IMyLightingBlock> light_dmg_tag = new List<IMyLightingBlock>();
        List<IMyBatteryBlock> battery_all = new List<IMyBatteryBlock>();
        List<IMyBatteryBlock> battery_tag = new List<IMyBatteryBlock>();
        List<IMyGasTank> hydrogen_tank_all = new List<IMyGasTank>();
        List<IMyGasTank> hydrogen_tank_tag = new List<IMyGasTank>();
        List<IMyShipDrill> drill_all = new List<IMyShipDrill>();
        List<IMyShipDrill> drill_tag = new List<IMyShipDrill>();
        List<MyWaypointInfo> waypoints = new List<MyWaypointInfo>();
        List<IMyThrust> thrust_all = new List<IMyThrust>();
        List<IMyThrust> thrust_tag = new List<IMyThrust>();
        List<IMyTerminalBlock> displays_all = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> displays_tag = new List<IMyTerminalBlock>();
        List<IMyTerminalBlock> displays_debug = new List<IMyTerminalBlock>();
        List<IMyProjector> projectors_all = new List<IMyProjector>();
        List<IMyEventControllerBlock> events_all = new List<IMyEventControllerBlock>();
        List<MyIGCMessage> syncMessagesBuffer = new List<MyIGCMessage>();
        IMyBlockGroup precModeGroup;
        IMyBlockGroup undockModeGroup;
        IMyBlockGroup resetModeGroup;
        IMyBlockGroup thrusterGroup;
        IMyGyro gyroActual;
        List<IMyGyro> gyro_all = new List<IMyGyro>();
        List<IMyGyro> gyroTag = new List<IMyGyro>();
        IMyShipDrill drl_act;
        IMyTextSurface visualiser;
        StringBuilder sb = new StringBuilder();
        MyIni _ini = new MyIni();
        bool setupIsComplete = false;
        double currentSpeed = 0.0;
        string n = "";
        bool Or_recall_1 = false;
        bool Or_recall_2 = false;
        string s_rc = "Remote Control";
        string s_ssr = "Sensor";
        string s_thr = "Thruster";
        string s_atmo = "Atmospheric";
        string s_hydro = "Hydrogen";
        string s_ion = "Ion";
        string s_proto = "Prototech";
        string s_antenna = "Antenna";
        string s_beacon = "Beacon";
        string s_camera = "Camera";
        string s_connector = "Connector";
        string s_battery = "Battery";
        string s_hydrogen_tank = "Hydrogen Tank";
        string s_oxygen_tank = "Oxygen Tank";
        string s_drill = "Drill";
        string s_gyroscope = "Gyroscope";
        string s_timerblock = "Timer Block";
        string s_flightmove = "AI Flight Move";
        string s_aitask = "AI Task Recorder";
        string s_lightblock = "Indication Light";
        string s_cargo = "Cargo Container";
        string s_display = "Display Panel";
        string s_eventctrl = "Event Controller";
        //string temp_id_name;
        int temp_id_num;
        double response_time = 0.0;
        double undock_delay_time = 0.0;
        double dock_delay_time = 0.0;
        double navigation_reset_delay_time = 0.0;
        string fail_data = "---:-1:0:0:0:0:0:0:0:";
        double spd;
        IMyBroadcastListener listn, listn_recall, listn_recall_drone, listn_png, listensync;
        MyIGCMessage new_msg, new_msg_2, new_msg_3, new_msg_4;
        string syncDataInput;
        Vector3D rc_xyz;
        float percent_battery_power = 0.0f;
        double pcnt_gas_tank = 0.0;
        double _Runtime;
        int _Instruction;
        bool dockingReady = false;
        bool thrustGroupPresent = false;
        bool precisionModeGroupPresent = false;
        bool undockModeGroupPresent = false;
        bool resetModeGroupPresent = false;
        bool switchedThrustersOff = false;
        bool switchedThrustersOn = false;
        bool batteryRechargeModeSet = false;
        bool batteryAutochargeSet = false;
        bool tankRechargeModeSet = false;
        bool tankAutochargeSet = false;
        string runargument = "";
        bool autoDocking = false;
        bool manualSenseAssign = false;
        bool gravityPresent = false;
        MyIni _sensorInfo = new MyIni();
        bool syncMessageReceived = false;
        bool secondary_tag_changed = false;
        //MyIni _commandData = new MyIni();
        MyIni _customDataStore = new MyIni();
        string gmdscategory = "GMDSJobData";
        string jobinfo = "Jobinfo";
        string jobdata = "";
        bool terrainclearEnable = false;
        bool terrainKeepMode = false;
        bool precMflip = false;
        bool precMflipped = false;
        string _recievedMessage;
        bool _updatedJob = false;
        StringBuilder sbtext = new StringBuilder();
        int runTick = 0;

        string D_S_C_Clone = "";
        string D_I_N_Clone = "";
        string D_C_N_Clone = "";
        string dockTaskName_Clone = "";
        string UndockModeTagName_Clone = "";
        string Thr_ON_n_Clone = "";
        string Thr_OFF_N_Clone = "";
        string precisionModeTagName_Clone = "";
        string ResetTagName_Clone = "";
        string C_A_T_N_Clone = "";
        string damageLightTagClone = "";
        bool initiateDrills = false;
        int gridrefreshcount = 0;
        bool AInav_fault = false;


        #endregion
        public void Save()
        {
            _ini.Clear();
            _ini.Set("configuration", "runargument", runargument);

            _ini.Set("configuration", "secondary tag", secondary_tag);
            _ini.Set("commands", "c1", recall);
            _ini.Set("commands", "c2", stopState);
            _ini.Set("commands", "c3", wasMining);
            _ini.Set("commands", "c4", navState);
            _ini.Set("commands", "c5", mineState);
            _ini.Set("commands", "c6", dockState);
            _ini.Set("commands", "c7", mode_set);

            _ini.Set("dockmode", "d1", dockingStage);
            _ini.Set("dockmode", "d2", undockState);
            _ini.Set("dockmode", "d3", undocking_start);
            _ini.Set("dockmode", "d4", undocking_stage);

            _ini.Set("unitstate", "u1", recharge_request);
            _ini.Set("unitstate", "u2", nav_act);
            _ini.Set("unitstate", "u3", mainNavSequence);
            _ini.Set("unitstate", "u4", main_nav_complete);
            _ini.Set("unitstate", "u5", add_nav_Waypoint_mn);
            _ini.Set("unitstate", "u6", miningInitialised);
            _ini.Set("unitstate", "u7", miningStage);
            _ini.Set("unitstate", "u8", add_mine_waypoint);
            _ini.Set("unitstate", "u9", mine_coords_adjusted);
            _ini.Set("unitstate", "u10", targetDepthAchieved);

            _ini.Set("unitstate", "u11", reset_mining);
            _ini.Set("unitstate", "u12", mining_nav_complete);
            _ini.Set("unitstate", "u13", force_request_dock);
            _ini.Set("unitstate", "u14", requestExit);
            _ini.Set("unitstate", "u15", exitSequenceComplete);
            _ini.Set("unitstate", "u16", exitWaypointSet);
            _ini.Set("unitstate", "u17", tunnelSequenceFinished);
            _ini.Set("unitstate", "u18", yawinst);
            _ini.Set("unitstate", "u19", pitchinst);
            _ini.Set("unitstate", "u20", rollinst);
            _ini.Set("unitstate", "u21", navinst);
            _ini.Set("unitstate", "u22", distance_current);

            _ini.Set("coordinates", "co1", mining_gps_coords.ToString().Trim());
            _ini.Set("coordinates", "co2", mining_gps_coords_temp.ToString().Trim());
            _ini.Set("coordinates", "co3", tgt_drill_start.ToString().Trim());
            _ini.Set("coordinates", "co4", tgt_drill_end.ToString().Trim());
            _ini.Set("coordinates", "co5", tgt_drill_exit.ToString().Trim());
            _ini.Set("coordinates", "co6", exit_gps_coords_temp.ToString().Trim());
            _ini.Set("coordinates", "co7", main_gps_coords.ToString().Trim());
            _ini.Set("coordinates", "co8", crnt_tgt_align.ToString().Trim());
            _ini.Set("coordinates", "co9", alignmentTargetNew.ToString().Trim());
            _ini.Set("coordinates", "co10", directionb.ToString().Trim());
            _ini.Set("coordinates", "co11", direction.ToString().Trim());
            _ini.Set("coordinates", "co12", directionc.ToString().Trim());
            _ini.Set("coordinates", "co13", gravity.ToString().Trim());
            _ini.Set("coordinates", "co14", gpsIndex.ToString().Trim());

            _ini.Set("coordinates", "drillterrainenabled", terrainclearEnable.ToString().Trim());
            _ini.Set("coordinates", "drillterrainoffset", terrainclearoffset.ToString().Trim());
            _ini.Set("coordinates", "keepterrain", terrainKeepMode.ToString().Trim());
            if (dataValid)
            {
                _ini.Set(gmdscategory, jobinfo, jobdata);
            }
            else
            {
                _ini.Set(gmdscategory, jobinfo, fail_data);
            }
            Storage = _ini.ToString();
            _ini.Clear();
        }

        void LoadStorageData(string input, string datacommandinput)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                Echo("No Storage data found.");
                //return;
            }
            _ini.Clear();
            if (_ini.TryParse(input))
            {
                var str = "";
                str = _ini.Get("configuration", "runargument").ToString().Trim();
                runargument = str;
                str = _ini.Get("configuration", "secondary tag").ToString();
                secondary_tag = str;
                str = _ini.Get("commands", "c1").ToString().Trim();
                bool.TryParse(str, out recall);
                str = _ini.Get("commands", "c2").ToString().Trim();
                bool.TryParse(str, out stopState);
                str = _ini.Get("commands", "c3").ToString().Trim();
                bool.TryParse(str, out wasMining);
                str = _ini.Get("commands", "c4").ToString().Trim();
                bool.TryParse(str, out navState);
                str = _ini.Get("commands", "c5").ToString().Trim();
                bool.TryParse(str, out mineState);
                str = _ini.Get("commands", "c6").ToString().Trim();
                bool.TryParse(str, out dockState);
                str = _ini.Get("commands", "c7").ToString().Trim();
                bool.TryParse(str, out mode_set);

                str = _ini.Get("dockmode", "d1").ToString().Trim();
                int.TryParse(str, out dockingStage);
                str = _ini.Get("dockmode", "d2").ToString().Trim();
                bool.TryParse(str, out undockState);
                str = _ini.Get("dockmode", "d3").ToString().Trim();
                int.TryParse(str, out undocking_start);
                str = _ini.Get("dockmode", "d4").ToString().Trim();
                int.TryParse(str, out undocking_stage);

                str = _ini.Get("unitstate", "u1").ToString().Trim();
                bool.TryParse(str, out recharge_request);
                str = _ini.Get("unitstate", "u2").ToString().Trim();
                bool.TryParse(str, out nav_act);
                str = _ini.Get("unitstate", "u3").ToString().Trim();
                int.TryParse(str, out mainNavSequence);
                str = _ini.Get("unitstate", "u4").ToString().Trim();
                bool.TryParse(str, out main_nav_complete);
                str = _ini.Get("unitstate", "u5").ToString().Trim();
                bool.TryParse(str, out add_nav_Waypoint_mn);
                str = _ini.Get("unitstate", "u6").ToString().Trim();
                bool.TryParse(str, out miningInitialised);
                str = _ini.Get("unitstate", "u7").ToString().Trim();
                int.TryParse(str, out miningStage);
                str = _ini.Get("unitstate", "u8").ToString().Trim();
                bool.TryParse(str, out add_mine_waypoint);
                str = _ini.Get("unitstate", "u9").ToString().Trim();
                bool.TryParse(str, out mine_coords_adjusted);
                str = _ini.Get("unitstate", "u10").ToString().Trim();
                bool.TryParse(str, out targetDepthAchieved);

                str = _ini.Get("unitstate", "u11").ToString().Trim();
                bool.TryParse(str, out reset_mining);
                str = _ini.Get("unitstate", "u12").ToString().Trim();
                bool.TryParse(str, out mining_nav_complete);
                str = _ini.Get("unitstate", "u13").ToString().Trim();
                bool.TryParse(str, out force_request_dock);
                str = _ini.Get("unitstate", "u14").ToString().Trim();
                bool.TryParse(str, out exitSequenceComplete);
                str = _ini.Get("unitstate", "u15").ToString().Trim();
                bool.TryParse(str, out exitWaypointSet);
                str = _ini.Get("unitstate", "u16").ToString().Trim();
                bool.TryParse(str, out tunnelSequenceFinished);
                str = _ini.Get("unitstate", "u18").ToString().Trim();
                bool.TryParse(str, out yawinst);
                str = _ini.Get("unitstate", "u19").ToString().Trim();
                bool.TryParse(str, out pitchinst);
                str = _ini.Get("unitstate", "u20").ToString().Trim();
                bool.TryParse(str, out rollinst);
                str = _ini.Get("unitstate", "u21").ToString().Trim();
                bool.TryParse(str, out navinst);
                str = _ini.Get("unitstate", "u22").ToString().Trim();
                double.TryParse(str, out distance_current);

                str = _ini.Get("coordinates", "co1").ToString().Trim();
                Vector3D.TryParse(str, out mining_gps_coords);
                str = _ini.Get("coordinates", "co2").ToString().Trim();
                Vector3D.TryParse(str, out mining_gps_coords_temp);
                str = _ini.Get("coordinates", "co3").ToString().Trim();
                Vector3D.TryParse(str, out tgt_drill_start);
                str = _ini.Get("coordinates", "co4").ToString().Trim();
                Vector3D.TryParse(str, out tgt_drill_end);
                str = _ini.Get("coordinates", "co5").ToString().Trim();
                Vector3D.TryParse(str, out tgt_drill_exit);
                str = _ini.Get("coordinates", "co6").ToString().Trim();
                Vector3D.TryParse(str, out exit_gps_coords_temp);
                str = _ini.Get("coordinates", "co7").ToString().Trim();
                Vector3D.TryParse(str, out main_gps_coords);
                str = _ini.Get("coordinates", "co8").ToString().Trim();
                Vector3D.TryParse(str, out crnt_tgt_align);
                str = _ini.Get("coordinates", "co9").ToString().Trim();
                Vector3D.TryParse(str, out alignmentTargetNew);
                str = _ini.Get("coordinates", "co10").ToString().Trim();
                Vector3D.TryParse(str, out directionb);
                str = _ini.Get("coordinates", "co11").ToString().Trim();
                Vector3D.TryParse(str, out direction);
                str = _ini.Get("coordinates", "co12").ToString().Trim();
                Vector3D.TryParse(str, out directionc);
                str = _ini.Get("coordinates", "co13").ToString().Trim();
                Vector3D.TryParse(str, out gravity);
                str = _ini.Get("coordinates", "co14").ToString().Trim();
                gpsIndex = str;
                str = _ini.Get("coordinates", "drillterrainenabled").ToString().Trim();
                if (!bool.TryParse(str, out terrainclearEnable))
                {
                    terrainclearEnable = false;
                }
                str = _ini.Get("coordinates", "keepterrain").ToString().Trim();
                if (!bool.TryParse(str, out terrainKeepMode))
                {
                    terrainKeepMode = false;
                }

                str = _ini.Get("coordinates", "drillterrainoffset").ToString().Trim();
                if (!double.TryParse(str, out terrainclearoffset))
                {
                    terrainclearoffset = 9.0;
                }
                str = _ini.Get(gmdscategory, jobinfo).ToString().Trim();
                if (!string.IsNullOrWhiteSpace(str))
                {
                    jobdata = str;
                }
            }

        }
        public void Main(string argument)
        {
            _Runtime = Runtime.LastRunTimeMs;
            _Instruction = Runtime.CurrentInstructionCount;

            // Check if a new argument was passed (manually via run or timer setup)
            if (!string.IsNullOrEmpty(argument) && !string.IsNullOrWhiteSpace(argument))
            {
                // --- Argument takes precedence for setup and override ---

                runargument = argument;
                ParseAndApplyArguments(argument);

                // Force a full setup if arguments changed
                setupIsComplete = false;
            }


            #region setup_code
            if (!setupIsComplete)
            {
                ClearAllNonEmptyLists();
                setup_function();
                setupIsComplete = true;
                item_presence_check();
                if (!setupIsComplete)
                {
                    Echo("Setup not complete.");
                    return;
                }
                confirm_item_presence();
                if (!setupIsComplete)
                {
                    Echo("Setup not complete.");
                    return;
                }
                Echo("Setup complete!");

                Save();
            }
            #endregion


            sbtext.Append("GMDS ").Append(ver).AppendLine(" Running...");

            bool canDock = (dockState);
            ClearCurrentWaypoints();
          //  Echo("yessir");
            item_presence_check();
         //   Echo("yessir2");
            confirm_item_presence();
           // Echo("yessir3");
            if (!setupIsComplete)
            {
                Echo("Setup not complete.");
                return;
            }


            if (runTick % 6 == 0)
            {
                cargo_check();
                damage_check();
            }
            if (runTick % 6 == 2)
            {
                power_check();
            }
            if (runTick % 6 == 4)
            {
                fuel_check();
            }
            runTick++;
            if (runTick > 60)
            {
                runTick = 0;
            }
            GetSpeed();
            recharge_state_check();
            terminationPrecisionUpdate();
            check_comms_channels();
            custom_data_command_presence_check(jobdata);
            command_poll();
            drone_operating_state_mng();
            connected_battery_recharge_check(dockingReady);
            DockingStateCheck();
            undock_management();
            dock_undock_state_check();
            drone_diver_state_management();
            check_for_planetary_gravity_presence();
            gravity_alignment_mng();
            check_ai_gravity_setting();
            drone_alignment_management();
            rc_navigation_init();
            if (navState)
            {
                navigation_management();
            }
            if (mineState || wasMining)
            {
                mining_management(autoDocking);
            }

            docking_management(canDock, autoDocking);
            connector_state_management(dockingReady);
            remote_control_position_update();
            drone_message_transmission_management(autoDocking, remoteControlActual, antenna_actual, dockingReady);
            nagivation_movement_check();
            undock_delay_check();
            dock_delay_check();
            GetDroneStatus(droneStatus);
            Drone_Local_Status_Reporting();
            function_delay_management();
        }
        private void confirm_item_presence()
        {
            if (!setupIsComplete)
            {
                sbtext.AppendLine("Drone parts missing - exiting");
                ClearAllNonEmptyLists();
                return;
            }
        }
        private void ClearCurrentWaypoints()
        {
            if (waypoints.Count > 0)
            {
                waypoints.Clear();
            }
        }
        Vector3D GetNavAngles(Vector3D Target)
        {
            if (remoteControlActual == null)
            {
                sbtext.AppendLine("No RC found");
                return Vector3D.Zero;
            }
            Vector3D RCcenter = remoteControlActual.GetPosition();
            Vector3D RCfow = remoteControlActual.WorldMatrix.Forward;
            Vector3D RCup = remoteControlActual.WorldMatrix.Up;
            Vector3D RCright = remoteControlActual.WorldMatrix.Right;
            Vector3D RCdown = -RCup;
            Vector3D targetDirection;


            if (targetAlignmentValid)
            {
                targetDirection = Vector3D.Normalize(Target - RCcenter);
            }
            else
            {
                targetDirection = remoteControlActual.GetNaturalGravity();
                if (targetDirection.LengthSquared() > 0)
                    targetDirection = Vector3D.Normalize(targetDirection);
                else
                    return Vector3D.Zero;
            }

            Vector3D rotationAxis = Vector3D.Cross(RCdown, targetDirection);
            double pitch = Vector3D.Dot(rotationAxis, RCright);

            double roll = Vector3D.Dot(rotationAxis, RCfow);

            double yaw = Vector3D.Dot(rotationAxis, RCup);

            return new Vector3D(yaw, roll, pitch);
        }

        private void terminationPrecisionUpdate()
        {
            termnationPrecision = (terminationCoefficient * drillSetLength) + 0.6;
        }
        void SetGyroOverride(bool OverrideOnOff, Vector3 settings, bool Parked, float Power = 1.0f)
        {
            if (gyroTag.Count > 0)
            {
                for (int j = 0; j < gyroTag.Count; j++)
                {
                    if (gyroTag[j] == null)
                    {
                        sbtext.AppendLine("Gyro [{i}] not found resetting setup flag");
                        setupIsComplete = false;
                    }
                    if (gyroTag[j] != null)
                    {
                        if (Parked && gyroTag[j].Enabled)
                        {
                            gyroTag[j].Enabled = false;
                        } else if (!Parked && !gyroTag[j].Enabled)
                        {
                            gyroTag[j].Enabled = true;
                        }
                        gyroActual = gyroTag[j];
                        if (gyroActual != null)
                        {
                            if (!gyroActual.GyroOverride && OverrideOnOff)
                            {
                                gyroActual.GyroOverride = true;
                            }
                            else if (gyroActual.GyroOverride && !OverrideOnOff)
                            {
                                gyroActual.GyroOverride = false;
                            }
                            if (gyroActual.GyroPower != Power)
                            {
                                gyroActual.GyroPower = Power;
                            }
                            if (gyroActual.Yaw != settings.GetDim(0))
                            {
                                gyroActual.Yaw = settings.GetDim(0);
                            }
                            if (gyroActual.Pitch != settings.GetDim(1))
                            {
                                gyroActual.Pitch = settings.GetDim(1);
                            }
                            if (gyroActual.Roll != settings.GetDim(2))
                            {
                                gyroActual.Roll = settings.GetDim(2);
                            }

                        }
                    }
                }
            }
        }

        void FetchJobData(string input)
        {
            _customDataStore.Clear();
            if (_customDataStore.TryParse(input))
            {
                var str = "";
                str = _customDataStore.Get(gmdscategory, jobinfo).ToString().Trim();
                jobdata = str;
            }
            _customDataStore.Clear();
        }

        void GetCustomDataCommand(string input, IMyTerminalBlock block)
        {
            // Checks if the block has CustomData AND if it's NOT already INI-formatted data
            if (!string.IsNullOrEmpty(block.CustomData) && !block.CustomData.Contains(gmdscategory))
            {
                _customDataStore.Clear();
                if (!_customDataStore.TryParse(Me.CustomData))
                {
                    String[] gpsCommandtest = block.CustomData.ToString().Split(':');

                    if (gpsCommandtest.Length > 0)
                    {
                        StoreRawInput(block.CustomData, block, gmdscategory, jobinfo);
                        _updatedJob = true;
                    }
                    sbtext.AppendLine("Dataconversion");
                }
            }
            if (string.IsNullOrEmpty(block.CustomData) || string.IsNullOrWhiteSpace(block.CustomData))
            {
                sbtext.AppendLine("Custom Data is empty");
                return;
            }
            // 2. Fetch and Initial Split
            FetchJobData(block.CustomData);
            string[] gpsData = jobdata.Split(':');
            int len = gpsData.Length;

            if (len < 5)
            {
                sbtext.AppendLine("Custom Data is faulty");
                jobdata = fail_data;
                return;
            }

            // 3. Main Target Parsing (Indices 1-7)
            gpsIndex = gpsData[1];

            // Direct Vector3D creation with parsing
            double mx = ParseDouble(gpsData, 2, 0.0);
            double my = ParseDouble(gpsData, 3, 0.0);
            double mz = ParseDouble(gpsData, 4, 0.0);
            main_gps_coords = new Vector3D(mx, my, mz);

            commandRequest = ParseInt(gpsData, 6, 0);
            drillSetLength = ParseDouble(gpsData, 7, 1.0);
            // 4. Distance and Extras (Indices 8-10)
            ignoreDistance = ParseDouble(gpsData, 8, 0.0);
            commandDataIgnoreDistance = gpsData[8];
            //commandData8 = (len > 9) ? gpsData[9] ?? "" : "";
            //commandData9 = (len > 10) ? gpsData[10] ?? "" : "";

            // 5. Alignment Logic (Indices 11-13)
            // We use the new helper to set both the value and the 'present' bool at once
            alignmentTargetX = ParseAndValidate(gpsData, 11, out commandDataPresent_11);
            alignmentTargetY = ParseAndValidate(gpsData, 12, out commandDataPresent_12);
            alignmentTargetZ = ParseAndValidate(gpsData, 13, out commandDataPresent_13);

            if (commandDataPresent_11 && commandDataPresent_12 && commandDataPresent_13)
            {
                targetAlignmentValid = true;
                alignmentTargetNew = new Vector3D(alignmentTargetX, alignmentTargetY, alignmentTargetZ);
            }
            else
            {
                targetAlignmentValid = false;
            }
        }

        void StDrlOnOff(bool DrilOnOf, bool UConv, bool terrainClear = false)
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
                        setupIsComplete = false;
                    }
                    if (drill_tag[i] != null)
                    {
                        drl_act = drill_tag[i];
                    }
                    if (drill_tag[i] != null)
                    {
                        if (DrilOnOf && !drl_act.Enabled)
                        {
                            drl_act.Enabled = true;
                        }
                        if (!DrilOnOf && drl_act.Enabled)
                        {
                            drl_act.Enabled = false;
                        }
                        if (terrainClear)
                        {
                            if (!drl_act.TerrainClearingMode)
                            {
                                drl_act.TerrainClearingMode = true;

                            }
                        }
                        else
                        {
                            if (drl_act.TerrainClearingMode)
                            {
                                drl_act.TerrainClearingMode = false;
                            }
                        }

                        if (UConv)
                        {
                            if (!drl_act.UseConveyorSystem)
                            {
                                drl_act.UseConveyorSystem = true;
                            }

                        }
                        else
                        {
                            if (drl_act.UseConveyorSystem)
                            {
                                drl_act.UseConveyorSystem = false;
                            }

                        }
                    }
                }
            }
        }
        void getnewdrills()
        {
            drill_all.Clear();
            drill_tag.Clear();
            GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(drill_all, b => b.CubeGrid == Me.CubeGrid);
            if (drill_all.Count > 0)
            {
                for (int i = 0; i < drill_all.Count; i++)
                {
                    if (drill_all[i].CustomName.Contains(D_I_N))
                    {
                        n = s_drill + " " + (i + 1) + " " + D_I_N;
                        drill_all[i].CustomName = n;
                        drill_tag.Add(drill_all[i]);
                    }
                    if (!drill_all[i].CustomName.Contains(D_I_N))
                    {
                        n = s_drill + " " + (i + 1) + " " + D_I_N;
                        drill_all[i].CustomName = n;
                        drill_tag.Add(drill_all[i]);
                    }
                }
            }
            drill_all.Clear();
        }

        void reset_ai()
        {
            //check ai move block and reset
            if (ai_move_actual != null)
            {
                if (!ai_move_actual.Enabled)
                {
                    ai_move_actual.Enabled = true;
                }
                if (ai_move_actual.GetValue<bool>(abr))
                {
                    ai_move_actual.GetActionWithName(ab0).Apply(ai_move_actual);
                }
            }
            //check ai task dock and undock and reset
            if (ai_task_dock_actual != null)
            {
                if (!ai_task_dock_actual.Enabled)
                {
                    ai_task_dock_actual.Enabled = true;
                }
                if (ai_task_dock_actual.GetValue<bool>(p1))
                {
                    ai_task_dock_actual.GetActionWithName(poff).Apply(ai_task_dock_actual);
                }
                if (ai_task_dock_actual.GetValue<bool>(abr))
                {
                    ai_task_dock_actual.GetActionWithName(ab0).Apply(ai_task_dock_actual);
                }
            }
            //check ai task undock and reset
            if (ai_task_undock_actual != null)
            {
                if (!ai_task_undock_actual.Enabled)
                {
                    ai_task_undock_actual.Enabled = true;
                }
                if (ai_task_undock_actual.GetValue<bool>(p1))
                {
                    ai_task_undock_actual.GetActionWithName(poff).Apply(ai_task_undock_actual);
                }
                if (ai_task_undock_actual.GetValue<bool>(abr))
                {
                    ai_task_undock_actual.GetActionWithName(ab0).Apply(ai_task_undock_actual);
                }
            }
            if (collisionAvoidLightActual != null)
            {
                if (collisionAvoidLightActual.Enabled)
                {
                    collisionAvoidLightActual.Enabled = false;
                }
            }
            if (precModeLightActual != null)
            {
                if (precModeLightActual.Enabled)
                {
                    precModeLightActual.Enabled = false;
                }
            }

            if (collisionSenseEnabled)
            {
                if (sensorActual != null)
                {
                    if (sensorActual.Enabled) { sensorActual.Enabled = false; }
                }
            }

        }

        private void ParseAndApplyArguments(string input)
        {
            // --- Step 1: Handle Empty Input (Using the simpler IsNullOrWhiteSpace check) ---
            if (string.IsNullOrWhiteSpace(input))
            {
                sbtext.AppendLine("No arguments provided, using defaults.");
                autoDocking = false;
                collisionSenseEnabled = true;
                cargoSenseEnabled = true;
                manualSenseAssign = false; // Initialize the new flag here
                droneTag = "UnassignedMiningDroneA";
                drone_id_num = 0;
                terrainclearoffset = 9.0;
                terrainKeepMode = false;
                drill_el = 20.0;
                exit_speed = 1.0f;
                nav_speed = 5.0f;
                drill_speed = 1.0f;
                return;
            }

            string[] droneconfigdata = input.Split(',');

            // Check if the split array is unexpectedly empty (though covered by the initial check)
            if (droneconfigdata.Length == 0)
            {
                sbtext.AppendLine("No arguments provided, using defaults.");
                // Use a consistent set of defaults or return immediately.
                return;
            }
            if (droneconfigdata.Length >= 1 && !string.IsNullOrWhiteSpace(droneconfigdata[0]))
            {
                droneTag = droneconfigdata[0].Trim();
            }
            else
            {
                droneTag = "UnassignedMiningDroneC"; // Default C if argument is missing or empty
            }
            if (droneconfigdata.Length >= 2 && !string.IsNullOrWhiteSpace(droneconfigdata[1]))
            {
                if (!int.TryParse(droneconfigdata[1].Trim(), out drone_id_num))
                {
                    drone_id_num = 0; // Set to default on fail
                }
            }
            else
            {
                drone_id_num = 0; // Default if argument is missing
            }

            if (droneconfigdata.Length > 2) //protect against index < 2
            {
                // Initialize flags to false (defaults)
                autoDocking = false;
                collisionSenseEnabled = false;
                cargoSenseEnabled = false;
                manualSenseAssign = false;
                terrainKeepMode = false;
                terrainclearoffset = 9.0;


                // Loop through all command arguments starting at index 2 (the first flag position) // do i need to test incorrect indexing on argument parse
                for (int i = 2; i < droneconfigdata.Length; i++)
                {
                    string arg = droneconfigdata[i].Trim().ToLower();

                    // Check for Auto Docking
                    if (arg.Contains(autodockCommand))
                    {
                        autoDocking = true;
                    }
                    // Check for Cargo Sense
                    if (arg.Contains(cargoSenseCommand))
                    {
                        cargoSenseEnabled = true;
                    }
                    // Check for Collision Sense
                    if (arg.Contains(collisionSenseCommand))
                    {
                        collisionSenseEnabled = true;
                    }
                    // Check for Manual Assign (New Feature)
                    if (arg.Contains(manualAssignCommand))
                    {
                        manualSenseAssign = true;
                    }
                    // Check for terrain saving mode (disable terrainclear)
                    if (arg.Contains(terrainClearCommand))
                    {
                        terrainKeepMode = true;
                    }
                    // Check for terrain clear distance command
                    if (arg.Contains(terrainDistanceCommand))
                    {

                        string temparg = arg;
                        if (!string.IsNullOrWhiteSpace(temparg))
                        {
                            String[] terraindistance = temparg.Split(':');
                            if (terraindistance.Length > 0)
                            {
                                if (terraindistance[0].Contains(terrainClearCommand))
                                {
                                    if (terraindistance.Length > 1)
                                    {
                                        if (!string.IsNullOrWhiteSpace(terraindistance[1]))
                                        {
                                            if (!double.TryParse(terraindistance[1], out terrainclearoffset))
                                            {
                                                terrainclearoffset = 9.0;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // Check for exit distance command
                    if (arg.Contains(exitDistanceCommand))
                    {

                        string temparg = arg;
                        if (!string.IsNullOrWhiteSpace(temparg))
                        {
                            String[] tempexitdistance = temparg.Split(':');
                            if (tempexitdistance.Length > 0)
                            {
                                if (tempexitdistance[0].Contains(exitDistanceCommand))
                                {
                                    if (tempexitdistance.Length > 1)
                                    {
                                        if (!string.IsNullOrWhiteSpace(tempexitdistance[1]))
                                        {
                                            if (!double.TryParse(tempexitdistance[1], out drill_el))
                                            {
                                                drill_el = 20.0;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // Check for exit speed command
                    if (arg.Contains(exitSpeedCommand))
                    {

                        string temparg = arg;
                        if (!string.IsNullOrWhiteSpace(temparg))
                        {
                            String[] tempexitspeed = temparg.Split(':');
                            if (tempexitspeed.Length > 0)
                            {
                                if (tempexitspeed[0].Contains(exitSpeedCommand))
                                {
                                    if (tempexitspeed.Length > 1)
                                    {
                                        if (!string.IsNullOrWhiteSpace(tempexitspeed[1]))
                                        {
                                            if (!float.TryParse(tempexitspeed[1], out exit_speed))
                                            {
                                                exit_speed = 1.0f;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // Check for nav speed command
                    if (arg.Contains(navSpeedCommand))
                    {

                        string temparg = arg;
                        if (!string.IsNullOrWhiteSpace(temparg))
                        {
                            String[] navspeedtemp = temparg.Split(':');
                            if (navspeedtemp.Length > 0)
                            {
                                if (navspeedtemp[0].Contains(navSpeedCommand))
                                {
                                    if (navspeedtemp.Length > 1)
                                    {
                                        if (!string.IsNullOrWhiteSpace(navspeedtemp[1]))
                                        {
                                            if (!float.TryParse(navspeedtemp[1], out nav_speed))
                                            {
                                                nav_speed = 1.0f;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // Check for drill speed command
                    if (arg.Contains(drillSpeedCommand))
                    {

                        string temparg = arg;
                        if (!string.IsNullOrWhiteSpace(temparg))
                        {
                            String[] drillspeedtemp = temparg.Split(':');
                            if (drillspeedtemp.Length > 0)
                            {
                                if (drillspeedtemp[0].Contains(drillSpeedCommand))
                                {
                                    if (drillspeedtemp.Length > 1)
                                    {
                                        if (!string.IsNullOrWhiteSpace(drillspeedtemp[1]))
                                        {
                                            if (!float.TryParse(drillspeedtemp[1], out drill_speed))
                                            {
                                                drill_speed = 1.0f;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        void GetDroneStatus(int drnstus)
        {
            #region void_drone_status_output
            if (drnstus == 0)
            {
                drnst = "Idle";
            }
            if (drnstus == 1 || drnstus == 4)
            {
                drnst = $"Nav CA {collisionSenseEnabled}";
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
            if (drnstus == 26)
            {
                drnst = "RTB Ready A";
            }
            if (drnstus == 27)
            {
                drnst = "RTB Ready B";
            }

            #endregion
        }
        public void drone_custom_data_check(string custominfo, int index)
        {
            bool load_id;
            bool load_tag;
            sbtext.AppendLine("Checking for drone config information..");
            String[] temp_id = custominfo.Split(':');
            sbtext.AppendLine($"{temp_id.Length}");

            if (temp_id.Length > 0)
            {
                if (temp_id[0] != null)
                {
                    if (!int.TryParse(temp_id[0], out temp_id_num))
                    {
                        temp_id_num = drone_id_num;
                        sbtext.Append("Resorting to default ID#.").Append(drone_id_num).Append('\n');

                    }
                    else
                    {
                        load_id = true;
                        if (load_id)
                        {
                            drone_id_num = temp_id_num;
                        }
                    }
                }

                if (temp_id[1] != null)
                {
                    //temp_id_name = temp_id[1];
                    load_tag = true;
                    if (load_tag)
                    {
                        droneTag = temp_id[1];
                    }
                    if (temp_id[1] == "" || temp_id[1] == null)
                    {
                        //temp_id_name = droneTag;
                        sbtext.AppendLine($"Resorting to default drone tag.{droneTag.Replace("[", "[[").Replace("]", "]]")}");
                    }
                }
            }


            if (temp_id.Length == 0)
            {
                temp_id_num = drone_id_num;
                //temp_id_name = droneTag;
                sbtext.AppendLine($"Resorting to default config. {droneTag.Replace("[", "[[").Replace("]", "]]")} {temp_id_num}");
            }

            if (antenna_all[index] != null)
            {
                antenna_all[index].CustomData = $"{drone_id_num}:{droneTag}";
            }
            sbtext.AppendLine($"Drone info: {drone_id_num}:{droneTag.Replace("[", "[[").Replace("]", "]]")}");
            D_I_N = $"[{droneTag} {drone_id_num}]";
            D_C_N = $"[{droneTag} {drone_id_num}]";
            dockTaskName = $"[{droneTag} {drone_id_num} {Dock}]";
            UndockModeTagName = $"[{droneTag} {drone_id_num} {Undock}]";
            Thr_ON_n = "[" + droneTag + " " + drone_id_num + " " + TON + "]";
            Thr_OFF_N = "[" + droneTag + " " + drone_id_num + " " + TOFF + "]";
            ResetTagName = $"[{droneTag} {drone_id_num} {Reset}]";
            CA_T_N = $"[{droneTag} {drone_id_num} {CA}]";
            PrecisionModeTagName = $"[{droneTag} {drone_id_num} {PrecM}]";
            H_T_N = $"[{droneTag} {drone_id_num} {HT}]";
            D_S_C = $"[{droneTag} {drone_id_num} {Sense}]";
            damageLightTag = $"[{droneTag} {drone_id_num} {dmg}]";
            pingChannel = $"[{droneTag}] {pingChannelTag}";
            thrustGroupTag = $"{thrusters} [{droneTag} {drone_id_num}]";
            tx_ch = droneTag + " reply";
            rx_channel_recall_drone = D_I_N + " " + recall_command;
            rx_channel_sync = "[" + droneTag + "]" + " " + syncChannelTag;
            S_N_T = $"[{secondary_tag}]";
            listensync = IGC.RegisterBroadcastListener(rx_channel_sync);
            Me.CustomName = $"GMDS Programmable Block {D_I_N} {S_N_T}";
            if (!Me.CubeGrid.CustomName.Contains(D_I_N))
            {
                var n = Me.CubeGrid.CustomName;
                Me.CubeGrid.CustomName = n + ($" - {D_I_N}");
            }
        }
        public void manageFirstLoad(string input, string datacommandinput)
        {
            if (!string.IsNullOrWhiteSpace(input) && !string.IsNullOrEmpty(input))
            {
                FetchJobData(Me.CustomData.ToString().Trim());
                //FetchConfigData(Me.CustomData.ToString().Trim());
                LoadStorageData(input, datacommandinput);
                ParseAndApplyArguments(runargument);
                StoreRawInput(jobdata, Me, gmdscategory, jobinfo);
                _updatedJob = true;
                sbtext.AppendLine("Configuration loaded from Storage.");
            }
            else
            {
                ParseAndApplyArguments(runargument);
                StoreRawInput(jobdata, Me, gmdscategory, jobinfo);
                _updatedJob = true;
                sbtext.AppendLine("No Storage data found, configuration loaded from arguments or defaults.");
            }

        }
        public void sensorrangemanagement(IMySensorBlock block)
        {
            if (string.IsNullOrEmpty(block.CustomData) || string.IsNullOrWhiteSpace(block.CustomData))
            {
                string input = "";
                /* 
                 *        float s_llm = 4.0f;
                    float s_rlm = 4.0f;
                float s_btlm = 3.0f;
                float s_tlm = 4.5f;
                float s_bklm = 6.5f;
                float s_flm = 3.0f;
                    */
                sbtext.AppendLine("No sensor range data found, using default.");
                _sensorInfo.Clear();
                _sensorInfo.Set("SensorRange", "LeftExtend", s_llm);
                _sensorInfo.Set("SensorRange", "RightExtend", s_rlm);
                _sensorInfo.Set("SensorRange", "BottomExtend", s_btlm);
                _sensorInfo.Set("SensorRange", "TopExtend", s_tlm);
                _sensorInfo.Set("SensorRange", "BackExtend", s_bklm);
                _sensorInfo.Set("SensorRange", "FrontExtend", s_flm);
                input = _sensorInfo.ToString();
                block.CustomData = input;
                _sensorInfo.Clear();
            }
            if (!string.IsNullOrEmpty(block.CustomData) && !string.IsNullOrWhiteSpace(block.CustomData))
            {
                sbtext.AppendLine("Sensor range data found, loading settings");
                if (_sensorInfo.TryParse(block.CustomData))
                {
                    _sensorInfo.TryParse(block.CustomData);
                    string senseval;
                    senseval = _sensorInfo.Get("SensorRange", "LeftExtend").ToString().Trim();
                    if (!float.TryParse(senseval, out s_llm))
                    {
                        s_llm = 1.0f;
                    }
                    else
                    {
                        float.TryParse(senseval, out s_llm);
                    }
                    senseval = _sensorInfo.Get("SensorRange", "RightExtend").ToString().Trim();
                    if (!float.TryParse(senseval, out s_rlm))
                    {
                        s_rlm = 1.0f;
                    }
                    else
                    {
                        float.TryParse(senseval, out s_rlm);
                    }
                    senseval = _sensorInfo.Get("SensorRange", "BottomExtend").ToString().Trim();
                    if (!float.TryParse(senseval, out s_btlm))
                    {
                        s_btlm = 1.0f;
                    }
                    else
                    {
                        float.TryParse(senseval, out s_btlm);
                    }
                    senseval = _sensorInfo.Get("SensorRange", "TopExtend").ToString().Trim();
                    if (!float.TryParse(senseval, out s_tlm
                            ))
                    {
                        s_tlm = 1.0f;
                    }
                    else
                    {
                        float.TryParse(senseval, out s_tlm);
                    }
                    senseval = _sensorInfo.Get("SensorRange", "BackExtend").ToString().Trim();
                    if (!float.TryParse(senseval, out s_bklm))
                    {
                        s_bklm = 1.0f;
                    }
                    else
                    {
                        float.TryParse(senseval, out s_bklm);
                    }
                    senseval = _sensorInfo.Get("SensorRange", "FrontExtend").ToString().Trim();
                    if (!float.TryParse(senseval, out s_flm))
                    {
                        s_flm = 1.0f;
                    }
                    else
                    {
                        float.TryParse(senseval, out s_flm);
                    }
                }
            }
        }

        public void blockRenamer()
        {
            S_N_T = $"[{secondary_tag}]";
            Me.CustomName = $"GMDS Programmable Block {D_I_N} {S_N_T}";
            if (!Me.CubeGrid.CustomName.Contains(D_I_N))
            {
                var n = Me.CubeGrid.CustomName;
                Me.CubeGrid.CustomName = n + ($" - {D_I_N}");
            }
            if (antenna_actual != null)
            {
                antenna_actual.HudText = $"{D_I_N} {S_N_T}";
                antenna_actual.ShowShipName = true;
            }
            if (beacons_tag.Count > 0)
            {
                if (beacons_tag[0] != null)
                {
                    beacons_tag[0].HudText = $"{D_I_N} {S_N_T}";
                }
            }
        }
        public void setup_function()
        {
            IMyGridTerminalSystem gts = GridTerminalSystem as IMyGridTerminalSystem;
            sb.Clear(); ;
            syncMessagesBuffer.Clear();
            if (string.IsNullOrEmpty(droneTag) || string.IsNullOrWhiteSpace(droneTag))
            {
                Echo($"Invalid name for drone_tag {droneTag.Replace("[", "[[").Replace("]", "]]")}");
                return;
            }
            tx_ch = droneTag + " reply";
            rx_channel_recall = droneTag + " " + recall_command;
            D_I_N = $"[{droneTag} {drone_id_num}]";
            D_C_N = $"[{droneTag} {drone_id_num}]";
            dockTaskName = $"[{droneTag} {drone_id_num} {Dock}]";
            UndockModeTagName = $"[{droneTag} {drone_id_num} {Undock}]";
            Thr_ON_n = "[" + droneTag + " " + drone_id_num + " " + TON + "]";
            Thr_OFF_N = "[" + droneTag + " " + drone_id_num + " " + TOFF + "]";
            ResetTagName = $"[{droneTag} {drone_id_num} {Reset}]";
            CA_T_N = $"[{droneTag} {drone_id_num} {CA}]";
            PrecisionModeTagName = $"[{droneTag} {drone_id_num} {PrecM}]";
            H_T_N = $"[{droneTag} {drone_id_num} {HT}]";
            D_S_C = $"[{droneTag} {drone_id_num} {Sense}]";
            damageLightTag = $"[{droneTag} {drone_id_num} {dmg}]";
            pingChannel = $"[{droneTag}] {pingChannelTag}";
            thrustGroupTag = $"{thrusters} [{droneTag} {drone_id_num}]";
            rx_channel_recall_drone = D_I_N + " " + recall_command;
            rx_channel_sync = "[" + droneTag + "]" + " " + syncChannelTag;
            S_N_T = $"[{secondary_tag}]";
            listensync = IGC.RegisterBroadcastListener(rx_channel_sync);

            Me.CustomName = $"GMDS Programmable Block {D_I_N} {S_N_T}";
            if (!Me.CubeGrid.CustomName.Contains(D_I_N))
            {
                var n = Me.CubeGrid.CustomName;
                Me.CubeGrid.CustomName = n + ($" - {D_I_N}");
            }

            //reset group presence
            thrustGroupPresent = false;
            precisionModeGroupPresent = false;
            undockModeGroupPresent = false;
            resetModeGroupPresent = false;

            //populate block lists
            antenna_all.Clear();
            antenna_tag.Clear();
            gts.GetBlocksOfType<IMyRadioAntenna>(antenna_all, b => b.CubeGrid == Me.CubeGrid);
            if (antenna_all.Count > 0)
            {
                for (int i = 0; i < antenna_all.Count; i++)
                {
                    if (antenna_all[i].CustomName.Contains(D_I_N))
                    {
                        string checker = antenna_all[i].CustomData;
                        //drone_custom_data_check(checker, i);
                        if (string.IsNullOrEmpty(droneTag) || string.IsNullOrWhiteSpace(droneTag))
                        {
                            Echo($"Invalid name for drone_tag {droneTag.Replace("[", "[[").Replace("]", "]]")}");
                            return;
                        }
                        n = s_antenna + " " + (i + 1) + " " + D_I_N;
                        antenna_all[i].CustomName = n;
                        antenna_all[i].HudText = $"{D_I_N} {S_N_T}";
                        antenna_all[i].ShowShipName = true;
                        antenna_tag.Add(antenna_all[i]);
                    }
                    if (!antenna_all[i].CustomName.Contains(D_I_N))
                    {
                        string checker = antenna_all[i].CustomData;
                        //drone_custom_data_check(checker, i);
                        if (string.IsNullOrEmpty(droneTag) || string.IsNullOrWhiteSpace(droneTag))
                        {
                            Echo($"Invalid name for drone_tag {droneTag.Replace("[", "[[").Replace("]", "]]")}");
                            return;
                        }
                        n = s_antenna + " " + (i + 1) + " " + D_I_N;
                        antenna_all[i].CustomName = n;
                        antenna_all[i].HudText = $"{D_I_N} {S_N_T}";
                        antenna_all[i].ShowShipName = true;
                        antenna_tag.Add(antenna_all[i]);
                    }
                }
            }
            antenna_all.Clear();
            rc_all.Clear();
            rctag.Clear();
            gts.GetBlocksOfType<IMyRemoteControl>(rc_all, b => b.CubeGrid == Me.CubeGrid);
            if (rc_all.Count > 0)
            {
                for (int i = 0; i < rc_all.Count; i++)
                {
                    if (rc_all[i].CustomName.Contains(D_I_N))
                    {
                        n = s_rc + " " + (i + 1) + " " + D_I_N;
                        rc_all[i].CustomName = n;
                        rctag.Add(rc_all[i]);
                    }
                    if (!rc_all[i].CustomName.Contains(D_I_N))
                    {
                        n = s_rc + " " + (i + 1) + " " + D_I_N;
                        rc_all[i].CustomName = n;
                        rctag.Add(rc_all[i]);
                    }

                }
            }
            rc_all.Clear();

            //getbeacons
            beacons_all.Clear();
            beacons_tag.Clear();
            gts.GetBlocksOfType<IMyBeacon>(beacons_all, b => b.CubeGrid == Me.CubeGrid);
            if (beacons_all.Count > 0)
            {
                for (int i = 0; i < beacons_all.Count; i++)
                {
                    if (beacons_all[i].CustomName.Contains(D_I_N))
                    {
                        n = s_beacon + " " + (i + 1) + " " + D_I_N;
                        beacons_all[i].CustomName = n;
                        beacons_all[i].HudText = $"{D_I_N} {S_N_T}";
                        beacons_tag.Add(beacons_all[i]);
                    }
                    if (!beacons_all[i].CustomName.Contains(D_I_N))
                    {
                        n = s_beacon + " " + (i + 1) + " " + D_I_N;
                        beacons_all[i].CustomName = n;
                        beacons_all[i].HudText = $"{D_I_N} {S_N_T}";
                        beacons_tag.Add(beacons_all[i]);
                    }

                }
            }
            beacons_all.Clear();
            if (collisionSenseEnabled)
            {
                sensor_all.Clear();
                sensor_tag.Clear();
                gts.GetBlocksOfType<IMySensorBlock>(sensor_all, b => b.CubeGrid == Me.CubeGrid);
                if (sensor_all.Count > 0)
                {
                    for (int i = 0; i < sensor_all.Count; i++)
                    {
                        if (sensor_all[i].CustomName.Contains(D_I_N))
                        {
                            n = s_ssr + " " + (i + 1) + " " + D_I_N;
                            sensor_all[i].CustomName = n;
                            sensor_tag.Add(sensor_all[i]);
                        }
                        if (!sensor_all[i].CustomName.Contains(D_I_N))
                        {
                            n = s_ssr + " " + (i + 1) + " " + D_I_N;
                            sensor_all[i].CustomName = n;
                            sensor_tag.Add(sensor_all[i]);
                        }
                    }
                }
                sensor_all.Clear();
                if (sensor_tag.Count <= 0 || sensor_tag[0] == null)
                {
                    Echo($"Sensor with tag: '{D_I_N.Replace("[", "[[").Replace("]", "]]")}' not found.");
                    return;
                }
                sensorActual = sensor_tag[0];

                sensorActual.DetectAsteroids = true;
                sensorActual.DetectEnemy = true;
                sensorActual.DetectFriendly = true;
                sensorActual.DetectLargeShips = true;
                sensorActual.DetectSmallShips = true;
                sensorActual.DetectSubgrids = true;
                sensorActual.DetectFloatingObjects = false;
                sensorActual.DetectStations = true;
                sensorActual.DetectPlayers = false;
                sensorActual.DetectNeutral = true;
                sensorActual.DetectOwner = true;
                sensorrangemanagement(sensorActual);
                sensorActual.LeftExtend = s_llm;
                sensorActual.RightExtend = s_rlm;
                sensorActual.BottomExtend = s_btlm;
                sensorActual.TopExtend = s_tlm;
                sensorActual.BackExtend = s_bklm;
                sensorActual.FrontExtend = s_flm;
            }
            cam_all.Clear();
            camera_tag.Clear();
            gts.GetBlocksOfType<IMyCameraBlock>(cam_all, b => b.CubeGrid == Me.CubeGrid);
            if (cam_all.Count > 0)
            {
                for (int i = 0; i < cam_all.Count; i++)
                {
                    if (cam_all[i].CustomName.Contains(D_I_N))
                    {
                        n = s_camera + " " + (i + 1) + " " + D_I_N;
                        cam_all[i].CustomName = n;
                        camera_tag.Add(cam_all[i]);
                    }
                    if (!cam_all[i].CustomName.Contains(D_I_N))
                    {
                        n = s_camera + " " + (i + 1) + " " + D_I_N;
                        cam_all[i].CustomName = n;
                        camera_tag.Add(cam_all[i]);
                    }
                }
            }
            cam_all.Clear();
            connector_all.Clear();
            connector_tag.Clear();
          //  refresh_tag.Clear();
            gts.GetBlocksOfType<IMyShipConnector>(connector_all, b => b.CubeGrid == Me.CubeGrid);
            if (connector_all.Count > 0)
            {
                for (int i = 0; i < connector_all.Count; i++)
                {
                    if (connector_all[i].CustomName.Contains(D_C_N) && !connector_all[i].CustomName.Contains("Refresh"))
                    {
                        n = s_connector + " " + (i + 1) + " " + D_I_N;
                        connector_all[i].CustomName = n;
                        connector_tag.Add(connector_all[i]);
                    }
                    if (!connector_all[i].CustomName.Contains(D_I_N) && !connector_all[i].CustomName.Contains("Refresh"))
                    {
                        n = s_connector + " " + (i + 1) + " " + D_I_N;
                        connector_all[i].CustomName = n;
                        connector_tag.Add(connector_all[i]);
                    }
                    /*    if (connector_all[i].CustomName.Contains("Refresh"))
                        {
                            n = s_connector + " Refresh " + (i + 1) + " " + D_I_N;
                            connector_all[i].CustomName = n;
                            connector_all[i].PullStrength = 0.0f;
                            refresh_tag.Add(connector_all[i]);
                        }
                     */
                }
            }
            connector_all.Clear();
            cargo_all.Clear();
            cargo_tag.Clear();
            cargo_sense = new List<IMyCargoContainer>();
            gts.GetBlocksOfType<IMyCargoContainer>(cargo_all, b => b.CubeGrid == Me.CubeGrid);
            if (cargo_all.Count > 0)
            {
                for (int i = 0; i < cargo_all.Count; i++)
                {
                    if (manualSenseAssign)
                    {
                        if (cargo_all[i].CustomName.Contains(D_I_N))
                        {
                            string tv1 = "";
                            if (cargo_all[i].BlockDefinition.SubtypeId.Contains("SmallBlockSmall") || cargo_all[i].BlockDefinition.SubtypeId.Contains("LargeBlockSmall"))
                            {
                                tv1 = "Small ";
                            }
                            if (cargo_all[i].BlockDefinition.SubtypeId.Contains("SmallBlockMedium"))
                            {
                                tv1 = "Medium ";
                            }
                            if (cargo_all[i].BlockDefinition.SubtypeId.Contains("LargeBlockLarge") || cargo_all[i].BlockDefinition.SubtypeId.Contains("SmallBlockLarge"))
                            {
                                tv1 = "Large ";
                            }
                            n = tv1 + s_cargo + " " + (i + 1) + " " + D_I_N;
                            cargo_all[i].CustomName = n;
                            cargo_tag.Add(cargo_all[i]);
                        }
                        if (cargo_all[i].CustomName.Contains(D_S_C))
                        {
                            string tv1 = "";
                            if (cargo_all[i].BlockDefinition.SubtypeId.Contains("SmallBlockSmall") || cargo_all[i].BlockDefinition.SubtypeId.Contains("LargeBlockSmall"))
                            {
                                tv1 = "Small ";
                            }
                            if (cargo_all[i].BlockDefinition.SubtypeId.Contains("SmallBlockMedium"))
                            {
                                tv1 = "Medium ";
                            }
                            if (cargo_all[i].BlockDefinition.SubtypeId.Contains("LargeBlockLarge") || cargo_all[i].BlockDefinition.SubtypeId.Contains("SmallBlockLarge"))
                            {
                                tv1 = "Large ";
                            }
                            n = tv1 + s_cargo + " " + (i + 1) + " " + D_I_N;
                            cargo_sense.Add(cargo_all[i]);
                        }
                        if (!cargo_all[i].CustomName.Contains(D_I_N) && !cargo_all[i].CustomName.Contains(D_S_C))
                        {
                            string tv1 = "";
                            if (cargo_all[i].BlockDefinition.SubtypeId.Contains("SmallBlockSmall") || cargo_all[i].BlockDefinition.SubtypeId.Contains("LargeBlockSmall"))
                            {
                                tv1 = "Small ";
                            }
                            if (cargo_all[i].BlockDefinition.SubtypeId.Contains("Medium"))
                            {
                                tv1 = "Medium ";
                            }
                            if (cargo_all[i].BlockDefinition.SubtypeId.Contains("LargeBlockLarge") || cargo_all[i].BlockDefinition.SubtypeId.Contains("SmallBlockLarge"))
                            {
                                tv1 = "Large ";
                            }
                            n = tv1 + s_cargo + " " + (i + 1) + " " + D_I_N;
                            cargo_all[i].CustomName = n + " " + D_I_N;
                            cargo_tag.Add(cargo_all[i]);
                        }
                    }
                    else
                    {
                        string tv1 = "";
                        if (cargo_all[i].BlockDefinition.SubtypeId.Contains("SmallBlockSmall") || cargo_all[i].BlockDefinition.SubtypeId.Contains("LargeBlockSmall"))
                        {
                            tv1 = "Small ";
                            n = tv1 + s_cargo + " " + (i + 1) + " " + D_I_N + " " + D_S_C;
                            cargo_all[i].CustomName = n + " " + S_N_T;
                            cargo_sense.Add(cargo_all[i]);
                        }
                        if (cargo_all[i].BlockDefinition.SubtypeId.Contains("Medium"))
                        {
                            tv1 = "Medium ";
                            n = tv1 + s_cargo + " " + (i + 1) + " " + D_I_N;
                            cargo_all[i].CustomName = n;
                            cargo_tag.Add(cargo_all[i]);
                        }
                        if (cargo_all[i].BlockDefinition.SubtypeId.Contains("LargeBlockLarge") || cargo_all[i].BlockDefinition.SubtypeId.Contains("SmallBlockLarge"))
                        {
                            tv1 = "Large ";
                            n = tv1 + s_cargo + " " + (i + 1) + " " + D_I_N;
                            cargo_all[i].CustomName = n;
                            cargo_tag.Add(cargo_all[i]);
                        }


                    }

                }
            }
            cargo_all.Clear();
            flight_path_all.Clear();
            flight_path_dock_tag.Clear();
            flight_path_undock_tag.Clear();
            gts.GetBlocksOfType<IMyPathRecorderBlock>(flight_path_all, b => b.CubeGrid == Me.CubeGrid);
            if (flight_path_all.Count > 0)
            {
                for (int i = 0; i < flight_path_all.Count; i++)
                {
                    if (flight_path_all[i].CustomName.Contains(dockTaskName) || flight_path_all[i].CustomName.Contains($" {Dock}"))
                    {
                        n = s_aitask + " Dock";
                        flight_path_all[i].CustomName = n + " " + (i + 1) + " " + dockTaskName;
                        flight_path_dock_tag.Add(flight_path_all[i]);
                    }
                    if (flight_path_all[i].CustomName.Contains(UndockModeTagName) || flight_path_all[i].CustomName.Contains($" {Undock}"))
                    {
                        n = s_aitask + " Undock";
                        flight_path_all[i].CustomName = n + " " + (i + 1) + " " + UndockModeTagName;
                        flight_path_undock_tag.Add(flight_path_all[i]);
                    }
                }
            }
            flight_path_all.Clear();
            flight_move_all.Clear();
            flight_move_tag.Clear();
            gts.GetBlocksOfType<IMyFlightMovementBlock>(flight_move_all, b => b.CubeGrid == Me.CubeGrid);
            if (flight_move_all.Count > 0)
            {
                for (int i = 0; i < flight_move_all.Count; i++)
                {
                    if (flight_move_all[i].CustomName.Contains(D_I_N))
                    {
                        n = s_flightmove;
                        flight_move_all[i].CustomName = n + " " + (i + 1) + " " + D_I_N;
                        flight_move_tag.Add(flight_move_all[i]);
                    }
                    if (!flight_move_all[i].CustomName.Contains(D_I_N))
                    {
                        n = s_flightmove;
                        flight_move_all[i].CustomName = n + " " + (i + 1) + " " + D_I_N;
                        flight_move_tag.Add(flight_move_all[i]);
                    }
                }
            }
            flight_move_all.Clear();
            thrust_all.Clear();
            thrust_tag.Clear();
            gts.GetBlocksOfType<IMyThrust>(thrust_all, b => b.CubeGrid == Me.CubeGrid);
            if (thrust_all.Count > 0)
            {
                for (int i = 0; i < thrust_all.Count; i++)
                {
                    if (thrust_all[i].CustomName.Contains(D_I_N))
                    {
                        string tv1 = "";
                        if (thrust_all[i].BlockDefinition.SubtypeId.Contains("Hydrogen"))
                        {
                            tv1 = s_hydro;
                        }
                        if (thrust_all[i].BlockDefinition.SubtypeId.Contains("Atmospheric"))
                        {
                            tv1 = s_atmo;
                        }
                        if (thrust_all[i].BlockDefinition.SubtypeId.Contains("LargeBlockLargeThrust") || thrust_all[i].BlockDefinition.SubtypeId.Contains("SmallBlockLargeThrust")
                            || thrust_all[i].BlockDefinition.SubtypeId.Contains("SmallBlockSmallThrust") || thrust_all[i].BlockDefinition.SubtypeId.Contains("LargeBlockSmallThrust")
                            || thrust_all[i].BlockDefinition.SubtypeId.Contains("ModularThruster")
                            )
                        {
                            tv1 = s_ion;
                        }
                        if (thrust_all[i].BlockDefinition.SubtypeId.Contains("LargeBlockPrototechThruster") || thrust_all[i].BlockDefinition.SubtypeId.Contains("SmallBlockPrototechThruster"))
                        {
                            tv1 = s_proto;
                        }
                        n = tv1 + " " + s_thr + " " + (i + 1) + " " + D_I_N;
                        thrust_all[i].CustomName = n;
                        thrust_tag.Add(thrust_all[i]);
                    }
                    if (!thrust_all[i].CustomName.Contains(D_I_N))
                    {
                        string tv1 = "";
                        if (thrust_all[i].BlockDefinition.SubtypeId.Contains("Hydrogen"))
                        {
                            tv1 = s_hydro;
                        }
                        if (thrust_all[i].BlockDefinition.SubtypeId.Contains("Atmospheric"))
                        {
                            tv1 = s_atmo;
                        }
                        if (thrust_all[i].BlockDefinition.SubtypeId.Contains("LargeBlockLargeThrust") || thrust_all[i].BlockDefinition.SubtypeId.Contains("SmallBlockLargeThrust")
                            || thrust_all[i].BlockDefinition.SubtypeId.Contains("SmallBlockSmallThrust") || thrust_all[i].BlockDefinition.SubtypeId.Contains("LargeBlockSmallThrust")
                            || thrust_all[i].BlockDefinition.SubtypeId.Contains("ModularThruster")
                            )
                        {
                            tv1 = s_ion;
                        }
                        if (thrust_all[i].BlockDefinition.SubtypeId.Contains("LargeBlockPrototechThruster") || thrust_all[i].BlockDefinition.SubtypeId.Contains("SmallBlockPrototechThruster"))
                        {
                            tv1 = s_proto;
                        }
                        n = tv1 + " " + s_thr + " " + (i + 1) + " " + D_I_N;
                        thrust_all[i].CustomName = n;
                        thrust_tag.Add(thrust_all[i]);
                    }
                }
            }
            thrust_all.Clear();

            thrusterGroup = gts.GetBlockGroupWithName(thrustGroupTag) as IMyBlockGroup;

            if (thrust_tag.Count > 0)
            {

                thrustGroupPresent = true;
                //thrusterGroup.GetBlocksOfType<IMyThrust>(thrust_tag, b => b.CubeGrid == Me.CubeGrid);
                thrust_tag.Clear();
                gts.GetBlocksOfType<IMyThrust>(thrust_tag, b => b.CubeGrid == Me.CubeGrid);
                Echo($"Thruster Group {thrustGroupTag.Replace("[", "[[").Replace("]", "]]")} found");
            }
            else
            {
                thrustGroupPresent = false;
                Echo($"Thruster Group {thrustGroupTag.Replace("[", "[[").Replace("]", "]]")} not found");
            }
            timer_block_all.Clear();
            timer_block_tON_tag.Clear();
            timer_block_tOFF_tag.Clear();
            timer_block_precM_tag.Clear();
            timer_block_undock_tag.Clear();
            gts.GetBlocksOfType<IMyTimerBlock>(timer_block_all, b => b.CubeGrid == Me.CubeGrid);
            if (timer_block_all.Count > 0)
            {
                for (int i = 0; i < timer_block_all.Count; i++)
                {
                    if (timer_block_all[i].CustomName.Contains(Thr_ON_n) || timer_block_all[i].CustomName.Contains(TON))
                    {
                        n = s_timerblock;
                        timer_block_all[i].CustomName = n + " " + (i + 1) + " " + Thr_ON_n;
                        timer_block_tON_tag.Add(timer_block_all[i]);
                    }
                    if (timer_block_all[i].CustomName.Contains(Thr_OFF_N) || timer_block_all[i].CustomName.Contains(TOFF))
                    {
                        n = s_timerblock;
                        timer_block_all[i].CustomName = n + " " + (i + 1) + " " + Thr_OFF_N;
                        timer_block_tOFF_tag.Add(timer_block_all[i]);
                    }
                    if (timer_block_all[i].CustomName.Contains(PrecisionModeTagName) || timer_block_all[i].CustomName.Contains(PrecM))
                    {
                        n = s_timerblock;
                        timer_block_all[i].CustomName = n + " " + (i + 1) + " " + PrecisionModeTagName;
                        timer_block_precM_tag.Add(timer_block_all[i]);
                    }
                    if (timer_block_all[i].CustomName.Contains(UndockModeTagName) || timer_block_all[i].CustomName.Contains($" {Undock}"))
                    {
                        n = s_timerblock;
                        timer_block_all[i].CustomName = n + " " + (i + 1) + " " + UndockModeTagName;
                        timer_block_undock_tag.Add(timer_block_all[i]);
                    }
                }
            }
            timer_block_all.Clear();
            light_all.Clear();
            lightUndockTag.Clear();
            light_dock_tag.Clear();
            light_collision_avoid_tag.Clear();
            lightPrecMTag.Clear();
            lightResetTag.Clear();
            light_dmg_tag.Clear();

            precModeGroup = gts.GetBlockGroupWithName(PrecisionModeTagName);
            if (precModeGroup != null)
            {
                precisionModeGroupPresent = true;
                Echo($"Precision mode group {PrecisionModeTagName.Replace("[", "[[").Replace("]", "]]")} found");
            }
            else
            {
                precisionModeGroupPresent = false;
                Echo($"Precision mode group {PrecisionModeTagName.Replace("[", "[[").Replace("]", "]]")} not found");
            }

            undockModeGroup = gts.GetBlockGroupWithName(UndockModeTagName);
            if (undockModeGroup != null)
            {
                undockModeGroupPresent = true;
                Echo($"Undock mode group {UndockModeTagName.Replace("[", "[[").Replace("]", "]]")} found");
            }
            else
            {
                undockModeGroupPresent = false;
                Echo($"Undock mode group {UndockModeTagName.Replace("[", "[[").Replace("]", "]]")} not found");
            }

            resetModeGroup = gts.GetBlockGroupWithName(ResetTagName);
            if (resetModeGroup != null)
            {
                resetModeGroupPresent = true;
                Echo($"Reset mode group {ResetTagName.Replace("[", "[[").Replace("]", "]]")} found");
            }
            else
            {
                resetModeGroupPresent = false;
                Echo($"Reset mode group {ResetTagName.Replace("[", "[[").Replace("]", "]]")} not found");
            }


            gts.GetBlocksOfType<IMyLightingBlock>(light_all, b => b.CubeGrid == Me.CubeGrid);
            if (light_all.Count > 0)
            {
                for (int i = 0; i < light_all.Count; i++)
                {
                    if (light_all[i].CustomName.Contains(dockTaskName) || light_all[i].CustomName.Contains($" {Dock}"))
                    {
                        n = s_lightblock;
                        light_all[i].CustomName = n + " " + (i + 1) + " " + dockTaskName;
                        light_dock_tag.Add(light_all[i]);
                    }
                    if (!undockModeGroupPresent && (light_all[i].CustomName.Contains(UndockModeTagName) || light_all[i].CustomName.Contains($" {Undock}")))
                    {
                        n = s_lightblock;
                        light_all[i].CustomName = n + " " + (i + 1) + " " + UndockModeTagName;
                        lightUndockTag.Add(light_all[i]);
                    }
                    if (light_all[i].CustomName.Contains(CA_T_N) || light_all[i].CustomName.Contains($" {CA}"))
                    {
                        n = s_lightblock;
                        light_all[i].CustomName = n + " " + (i + 1) + " " + CA_T_N;
                        light_collision_avoid_tag.Add(light_all[i]);
                    }
                    if (!resetModeGroupPresent && (light_all[i].CustomName.Contains(ResetTagName) || light_all[i].CustomName.Contains($" {Reset}")))
                    {
                        n = s_lightblock;
                        light_all[i].CustomName = n + " " + (i + 1) + " " + ResetTagName;
                        lightResetTag.Add(light_all[i]);
                    }

                    if (!precisionModeGroupPresent && (light_all[i].CustomName.Contains(PrecisionModeTagName) || light_all[i].CustomName.Contains($" {PrecM}")))
                    {
                        n = s_lightblock;
                        light_all[i].CustomName = n + " " + (i + 1) + " " + PrecisionModeTagName;
                        lightPrecMTag.Add(light_all[i]);
                    }
                    if (light_all[i].CustomName.Contains(damageLightTag) || light_all[i].CustomName.Contains($" {dmg}"))
                    {
                        n = s_lightblock;
                        light_all[i].CustomName = n + " " + (i + 1) + " " + damageLightTag;
                        light_dmg_tag.Add(light_all[i]);
                    }
                }
                if (precisionModeGroupPresent)
                {
                    precModeGroup.GetBlocksOfType<IMyLightingBlock>(lightPrecMTag, b => b.CubeGrid == Me.CubeGrid);
                    if (lightPrecMTag.Count > 0)
                    {
                        for (int i = 0; i < lightPrecMTag.Count; i++)
                        {
                            n = s_lightblock;
                            lightPrecMTag[i].CustomName = n + " " + (i + 1) + " " + PrecisionModeTagName;
                        }

                    }
                }
                if (undockModeGroupPresent)
                {
                    undockModeGroup.GetBlocksOfType<IMyLightingBlock>(lightUndockTag, b => b.CubeGrid == Me.CubeGrid);
                    if (lightUndockTag.Count > 0)
                    {
                        for (int i = 0; i < lightUndockTag.Count; i++)
                        {
                            n = s_lightblock;
                            lightUndockTag[i].CustomName = n + " " + (i + 1) + " " + UndockModeTagName;
                        }

                    }
                }
                if (resetModeGroupPresent)
                {
                    resetModeGroup.GetBlocksOfType<IMyLightingBlock>(lightResetTag, b => b.CubeGrid == Me.CubeGrid);
                    if (lightResetTag.Count > 0)
                    {
                        for (int i = 0; i < lightResetTag.Count; i++)
                        {
                            n = s_lightblock;
                            lightResetTag[i].CustomName = n + " " + (i + 1) + " " + ResetTagName;
                        }

                    }
                }
            }
            light_all.Clear();
            battery_all.Clear();
            battery_tag.Clear();
            gts.GetBlocksOfType<IMyBatteryBlock>(battery_all, b => b.CubeGrid == Me.CubeGrid);
            if (battery_all.Count > 0)
            {
                for (int i = 0; i < battery_all.Count; i++)
                {
                    if (battery_all[i].CustomName.Contains(D_I_N))
                    {
                        string tv1 = "";
                        if (battery_all[i].CustomName.Contains("Small"))
                        {
                            tv1 = "Small";
                        }
                        if (battery_all[i].CustomName.Contains("Medium"))
                        {
                            tv1 = "Medium";
                        }
                        n = tv1 + " " + s_battery + " " + (i + 1) + " " + D_I_N;
                        battery_all[i].CustomName = n;
                        battery_tag.Add(battery_all[i]);
                    }
                    if (!battery_all[i].CustomName.Contains(D_I_N))
                    {
                        string tv1 = "";
                        if (battery_all[i].CustomName.Contains("Small"))
                        {
                            tv1 = "Small";
                        }
                        if (battery_all[i].CustomName.Contains("Medium"))
                        {
                            tv1 = "Medium";
                        }
                        n = tv1 + " " + s_battery + " " + (i + 1) + " " + D_I_N;
                        battery_all[i].CustomName = n;
                        battery_tag.Add(battery_all[i]);
                    }
                }
            }
            battery_all.Clear();
            hydrogen_tank_all.Clear();
            hydrogen_tank_tag.Clear();
            gts.GetBlocksOfType<IMyGasTank>(hydrogen_tank_all, b => b.CubeGrid == Me.CubeGrid);
            if (hydrogen_tank_all.Count > 0)
            {
                for (int i = 0; i < hydrogen_tank_all.Count; i++)
                {
                    if (hydrogen_tank_all[i].CustomName.Contains(H_T_N) && hydrogen_tank_all[i].BlockDefinition.SubtypeId.Contains("HydrogenTank"))
                    {
                        n = s_hydrogen_tank + " " + (i + 1) + " " + H_T_N;
                        hydrogen_tank_all[i].CustomName = n;
                        hydrogen_tank_tag.Add(hydrogen_tank_all[i]);
                    }
                    if (!hydrogen_tank_all[i].CustomName.Contains(H_T_N) && hydrogen_tank_all[i].BlockDefinition.SubtypeId.Contains("HydrogenTank"))
                    {
                        n = s_hydrogen_tank + " " + (i + 1) + " " + H_T_N;
                        hydrogen_tank_all[i].CustomName = n;
                        hydrogen_tank_tag.Add(hydrogen_tank_all[i]);
                    }
                    if (!hydrogen_tank_all[i].BlockDefinition.SubtypeId.Contains("HydrogenTank"))
                    {
                        n = s_oxygen_tank + " " + (i + 1) + " " + H_T_N;
                        hydrogen_tank_all[i].CustomName = n;
                    }
                }
            }
            hydrogen_tank_all.Clear();
            drill_all.Clear();
            drill_tag.Clear();
            gts.GetBlocksOfType<IMyShipDrill>(drill_all, b => b.CubeGrid == Me.CubeGrid);
            if (drill_all.Count > 0)
            {
                for (int i = 0; i < drill_all.Count; i++)
                {
                    if (drill_all[i].CustomName.Contains(D_I_N))
                    {
                        n = s_drill + " " + (i + 1) + " " + D_I_N;
                        drill_all[i].CustomName = n;
                        drill_tag.Add(drill_all[i]);
                    }
                    if (!drill_all[i].CustomName.Contains(D_I_N))
                    {
                        n = s_drill + " " + (i + 1) + " " + D_I_N;
                        drill_all[i].CustomName = n;
                        drill_tag.Add(drill_all[i]);
                    }
                }
            }
            drill_all.Clear();
            gyro_all.Clear();
            gyroTag.Clear();
            gts.GetBlocksOfType<IMyGyro>(gyro_all, b => b.CubeGrid == Me.CubeGrid);
            if (gyro_all.Count > 0)
            {
                for (int i = 0; i < gyro_all.Count; i++)
                {
                    if (gyro_all[i].CustomName.Contains(D_I_N))
                    {
                        n = s_gyroscope + " " + (i + 1) + " " + D_I_N;
                        gyro_all[i].CustomName = n;
                        gyroTag.Add(gyro_all[i]);
                    }
                    if (!gyro_all[i].CustomName.Contains(D_I_N))
                    {
                        n = s_gyroscope + " " + (i + 1) + " " + D_I_N;
                        gyro_all[i].CustomName = n;
                        gyroTag.Add(gyro_all[i]);
                    }
                }
            }
            gyro_all.Clear();
            //display search
            displays_all.Clear();
            displays_tag.Clear();
            displays_debug.Clear();
            gts.GetBlocksOfType<IMyTerminalBlock>(displays_all, b => b.CubeGrid == Me.CubeGrid);
            if (displays_all.Count > 0)
            {
                for (int i = 0; i < displays_all.Count; i++)
                {
                    string tv1 = "";
                    if (
                        (displays_all[i].BlockDefinition.SubtypeId.Contains("TextPanel") && !displays_all[i].Name.Contains(debug) && displays_all[i].Name.Contains(display))
                        || (!displays_all[i].BlockDefinition.SubtypeId.Contains("TextPanel") && !displays_all[i].Name.Contains(debug) && displays_all[i].Name.Contains(display))
                        )
                    {
                        tv1 = s_display;
                        n = tv1 + s_display + " " + (i + 1) + " " + D_I_N + " " + display;
                        displays_all[i].CustomName = n;
                        displays_tag.Add(displays_all[i]);
                    }
                    if (displays_all[i].BlockDefinition.SubtypeId.Contains("TextPanel") && displays_all[i].Name.Contains(debug) && !displays_all[i].Name.Contains(display))
                    {
                        tv1 = s_display;
                        n = tv1 + s_display + " " + (i + 1) + " " + D_I_N + " " + debug;
                        displays_all[i].CustomName = n;
                        displays_debug.Add(displays_all[i]);
                    }
                    if (displays_all[i].BlockDefinition.SubtypeId.Contains("TextPanel") && !displays_all[i].Name.Contains(debug) && !displays_all[i].Name.Contains(display))
                    {
                        tv1 = s_display;
                        n = tv1 + s_display + " " + (i + 1) + " " + D_I_N + " " + display;
                        displays_all[i].CustomName = n;
                        displays_tag.Add(displays_all[i]);
                    }
                }
            }
            displays_all.Clear();

            projectors_all.Clear();
            gts.GetBlocksOfType<IMyProjector>(projectors_all, b => b.CubeGrid == Me.CubeGrid);
            if (projectors_all.Count > 0)
            {
                for (int i = 0; i < projectors_all.Count; i++)
                {
                    string tv1 = "";
                    tv1 = "Projector";
                    n = tv1 + " " + (i + 1) + " " + D_I_N;
                    projectors_all[i].CustomName = n;
                }
            }
            projectors_all.Clear();
            events_all.Clear();
            gts.GetBlocksOfType<IMyEventControllerBlock>(events_all, b => b.CubeGrid == Me.CubeGrid);
            if(events_all.Count > 0)
            {
                for(int i = 0; i < events_all.Count; i++)
                {
                    n = s_eventctrl + " " + (i + 1) + " " + D_I_N;
                    events_all[i].CustomName = n;
                }
                   
            }
            events_all.Clear();
            waypoints.Clear();
            
            /* if (Storage != "" && Storage != null)
             {
                 LoadStorageData();
                 Storage = "";
             } */ //should be handled in main program

            #region setup_broadcast_channels
            rx_ch = D_I_N;
            listn = IGC.RegisterBroadcastListener(rx_ch);
            listn_recall = IGC.RegisterBroadcastListener(rx_channel_recall);
            listn_recall_drone = IGC.RegisterBroadcastListener(rx_channel_recall_drone);
            listn_png = IGC.RegisterBroadcastListener(pingChannel);
            listensync = IGC.RegisterBroadcastListener(rx_channel_sync);
            #endregion
            D_S_C_Clone = D_S_C.Replace("[", "[[").Replace("]", "]]");
            D_I_N_Clone = D_I_N.Replace("[", "[[").Replace("]", "]]");
            D_C_N_Clone = D_C_N.Replace("[", "[[").Replace("]", "]]");
            dockTaskName_Clone = dockTaskName.Replace("[", "[[").Replace("]", "]]");
            UndockModeTagName_Clone = UndockModeTagName.Replace("[", "[[").Replace("]", "]]");
            Thr_ON_n_Clone = Thr_ON_n.Replace("[", "[[").Replace("]", "]]");
            Thr_OFF_N_Clone = Thr_OFF_N.Replace("[", "[[").Replace("]", "]]");
            precisionModeTagName_Clone = PrecisionModeTagName.Replace("[", "[[").Replace("]", "]]");
            ResetTagName_Clone = ResetTagName.Replace("[", "[[").Replace("]", "]]");
            C_A_T_N_Clone = CA_T_N.Replace("[", "[[").Replace("]", "]]");
            damageLightTagClone = damageLightTag.Replace("[", "[[").Replace("]", "]]");
        }
        public void item_presence_check()
        {
            if (!setupIsComplete)
            {
                Echo("Setup not complete.");
                return;
            }

            #region presence_check

            if (thrust_tag.Count <= 0 && thrustGroupPresent)
            {
                Echo("Please add thrusters to grid");
                return;
            }

            if (drill_tag.Count <= 0)
            {
                Echo($"Drills with tag: '{D_I_N_Clone}' not found.");
                return;
            }

            // Begin Validations & Assignments
            if (ValidateTag(gyroTag, $"Gyro with tag: '{D_I_N_Clone}' not found.") == null) return;

            remoteControlActual = ValidateTag(rctag, $"Remote control with tag: '{D_I_N_Clone}' not found.");
            if (remoteControlActual == null) return;

            if (collisionSenseEnabled)
            {
                sensorActual = ValidateTag(sensor_tag, $"Sensor with tag: '{D_I_N_Clone}' not found.");
                if (sensorActual == null) return;
            }

            if (ValidateTag(camera_tag, $"Camera with tag: '{D_I_N_Clone}' not found.", false) == null) return;

            connectorActual = ValidateTag(connector_tag, $"Connector with tag: '{D_C_N_Clone}' not found.");
            if (connectorActual == null) return;

            if (ValidateTag(cargo_tag, $"Cargo containers with tag: '{D_I_N_Clone}' not found.") == null) return;

            if (cargoSenseEnabled && (cargo_sense == null || cargo_sense.Count <= 0 || cargo_sense[0] == null))
            {
                Echo($"Sense container with tag: '{D_S_C_Clone}' not found. Add '{D_S_C_Clone}' tag to container");
            }

            antenna_actual = ValidateTag(antenna_tag, $"Antenna with tag: '{D_I_N_Clone}' not found.");
            if (antenna_actual == null) return;

            ai_task_dock_actual = ValidateTag(flight_path_dock_tag, $"Docking AI task recorder with tag: '{dockTaskName_Clone}' not found. Add ' {Dock}' tag");
            if (ai_task_dock_actual == null) return;

            ai_task_undock_actual = ValidateTag(flight_path_undock_tag, $"Undocking AI task recorder with tag: '{UndockModeTagName_Clone}' not found. Add ' {Undock}' tag");
            if (ai_task_undock_actual == null) return;

            ai_move_actual = ValidateTag(flight_move_tag, $"Flight movement with tag: '{D_I_N_Clone}' not found.");
            if (ai_move_actual == null) return;

            if (!thrustGroupPresent)
            {
                timerBlockTONActual = ValidateTag(timer_block_tON_tag, $"Thrust ON timer block with tag: '{Thr_ON_n_Clone}' not found. Add ' {TON}' tag");
                if (timerBlockTONActual == null) return;

                timerBlockTOFFActual = ValidateTag(timer_block_tOFF_tag, $"Thrust OFF timer block with tag: '{Thr_OFF_N_Clone}' not found. Add ' {TOFF}' tag");
                if (timerBlockTOFFActual == null) return;
            }

            if (!precisionModeGroupPresent && ValidateTag(timer_block_precM_tag, $"Precision mode timer block with tag: '{precisionModeTagName_Clone}' not found. Add ' {PrecM}' tag") == null) return;

            if (!undockModeGroupPresent && ValidateTag(timer_block_undock_tag, $"Undock mode timer block with tag: '{UndockModeTagName_Clone}' not found. Add ' {Undock}' tag") == null) return;

            dockLightActual = ValidateTag(light_dock_tag, $"dock indicator light with tag: '{dockTaskName_Clone}' not found. Add ' {Dock}' tag");
            if (dockLightActual == null) return;

            // Error message logic using C# 6.0 ternary operators
            string undockErrorMsg = undockModeGroupPresent
                ? $"Add undock indicator light with tag: '{UndockModeTagName_Clone}' to {UndockModeTagName_Clone} group - ensure {UndockModeTagName_Clone} group is in AI {UndockModeTagName_Clone} task recorder waypoint actions"
                : $"undock indicator light with tag: '{UndockModeTagName_Clone}' not found. Add ' {Undock}' tag";
            undockLightActual = ValidateTag(lightUndockTag, undockErrorMsg);
            if (undockLightActual == null) return;

            collisionAvoidLightActual = ValidateTag(light_collision_avoid_tag, $"collision avoidance required indicator light with tag: '{C_A_T_N_Clone}' not found. Add ' {CA}' tag");
            if (collisionAvoidLightActual == null) return;

            string precErrorMsg = precisionModeGroupPresent
                ? $"Add precision mode indicator light with tag: '{precisionModeTagName_Clone}' to {precisionModeTagName_Clone} group - ensure {precisionModeTagName_Clone} group is in AI {dockTaskName_Clone} task recorder waypoint actions"
                : $"Precision mode required indicator light with tag: '{precisionModeTagName_Clone}' not found. Add ' {PrecM}' tag";
            precModeLightActual = ValidateTag(lightPrecMTag, precErrorMsg);
            if (precModeLightActual == null) return;

            string resetErrorMsg = resetModeGroupPresent
                ? $"Reset mode indicator light with tag: '{ResetTagName_Clone}' to {ResetTagName_Clone} group - ensure {ResetTagName_Clone} group is in Sensor {D_I_N_Clone} detect action only"
                : $"Dock reset indicator light with tag: '{ResetTagName_Clone}' not found. Add ' {Reset}' tag";
            resetLightActual = ValidateTag(lightResetTag, resetErrorMsg);
            if (resetLightActual == null) return;

            if (damageReportingEnabled)
            {
                damageLightActual = ValidateTag(light_dmg_tag, $"Damage indicator light with tag: '{damageLightTagClone}' not found. Add ' {dmg}' tag\n", false);
            }

            if (battery_tag == null || battery_tag.Count <= 0)
            {
                Echo($"Batteries with tag: '{D_I_N_Clone}' not found.");
                setupIsComplete = false;
                return;
            }

            #endregion
        }

        // Move this to the class level (outside of item_presence_check)
        private T ValidateTag<T>(List<T> list, string errorMessage, bool failSetup = true) where T : class
        {
            if (list == null || list.Count <= 0 || list[0] == null)
            {
                Echo(errorMessage);
                if (failSetup) setupIsComplete = false;
                return null;
            }
            return list[0];
        }

        public void cargo_check()
        {
            #region cargo_check
            float ttl_volu = 0.0f;
            float ttl_volm = 0.0f;
            total_percent_cargo_used = 0;
            for (int i = 0; i < cargo_tag.Count; i++)
            {
                if (cargo_tag[i] != null)
                {
                    if (cargo_tag[i].IsFunctional)
                    {
                        float inventory_vol = (float)cargo_tag[i].GetInventory(0).CurrentVolume;
                        float max_inventory_vol = (float)cargo_tag[i].GetInventory(0).MaxVolume;
                        ttl_volu += inventory_vol;
                        ttl_volm += max_inventory_vol;
                    }
                }
                else
                {
                    sbtext.AppendLine($"Warning: Cargo container [{i}] is null in cargo_check");
                }
            }
            if (ttl_volm > 0.0f)
            {
                total_percent_cargo_used = (ttl_volu / ttl_volm) * 100;
            }
            else
            {
                total_percent_cargo_used = 0.0f;
            }
            //
            if (total_percent_cargo_used == 100.0f)
            {
                cargoIsFull = true;
            }
            if (total_percent_cargo_used < 100.0f)
            {
                cargoIsFull = false;
            }
            if (total_percent_cargo_used == 0.0f)
            {
                cargoIsEmpty = true;
            }
            if (total_percent_cargo_used > 0.0f)
            {
                cargoIsEmpty = false;
            }
            if (cargoIsEmpty && cargoFullAchieved)
            {
                cargoFullAchieved = false;
            }
            if (cargoSenseEnabled && cargo_sense.Count > 0) // Only run if sense container exists
            {
                float ttl_volus = 0.0f;
                float ttl_volms = 0.0f;
                float ttl_pctus = 0.0f;
                for (int i = 0; i < cargo_sense.Count; i++)
                {
                    if (cargo_sense[i] != null)
                    {
                        if (cargo_tag[i].IsFunctional)
                        {
                            float inventory_vol_s = (float)cargo_sense[i].GetInventory(0).CurrentVolume;
                            float max_inventory_vol_s = (float)cargo_sense[i].GetInventory(0).MaxVolume;
                            ttl_volus += inventory_vol_s;
                            ttl_volms += max_inventory_vol_s;
                        }
                    }
                    else
                    {
                        sbtext.AppendLine($"Warning: Sense cargo container [{i}] is null in cargo_check");
                    }
                }
                if (ttl_volms > 0.0f)
                {
                    ttl_pctus = (ttl_volus / ttl_volms) * 100;
                }
                else
                {
                    ttl_pctus = 0.0f;
                }
                sens_convOPN = (ttl_pctus > cargoSenseLimit);
            }
            else
            {
                sens_convOPN = false; // Default if no sense container
            }
            #endregion
        }

        public void remote_control_position_update()
        {
            if (remoteControlActual == null)
            {
                sbtext.AppendLine("Remote control is null in remote_control_position_update");
                return;
            }
            rc_xyz = remoteControlActual.GetPosition();

        }

        public void GetSpeed()
        {
            if (remoteControlActual != null)
            {
                spd = remoteControlActual.GetShipSpeed();
                currentSpeed = spd;
            }

        }

        public void damage_check()
        {

            #region damage_check
            if (!damageReportingEnabled)
            {
                droneDamageStatus = "OK";
            }
            if (damageReportingEnabled)
            {
                if (damageLightActual == null && damageReportingEnabled)
                {
                    droneDamageStatus = "UNK";
                    sbtext.AppendLine("Warning: Damage light is null in damage check");
                    return;
                }
                if (damageLightActual != null)
                {
                    if (damageLightActual.Enabled && damageReportingEnabled && damageLightActual.IsFunctional || damageReportingEnabled && !damageLightActual.IsFunctional)
                    {
                        droneDamageStatus = "DMG";
                    }
                    if (!damageLightActual.Enabled && damageReportingEnabled && damageLightActual.IsFunctional)
                    {
                        droneDamageStatus = "OK";
                    }
                }
            }
            #endregion

        }

        public void power_check()
        {

            #region power_check
            ttl_sPWR = 0f;  // Total stored power
            ttl_mPWR = 0f;  // Total max power
                            // ttl_PWRc = 0f;  // Total current output
            int validBatteries = 0;

            // Single loop to gather totals and optionally set ChargeMode
            for (int i = 0; i < battery_tag.Count; i++)
            {
                if (battery_tag[i] != null)
                {
                    if (battery_tag[i].IsFunctional)
                    {
                        crntbatteryblock = battery_tag[i];
                        ttl_sPWR += crntbatteryblock.CurrentStoredPower;
                        ttl_mPWR += crntbatteryblock.MaxStoredPower;
                        //ttl_PWRc += crntbatteryblock.CurrentOutput;
                        validBatteries++;
                    }

                    // Set ChargeMode in the same loop if conditions allow
                    /*   if (connectorActual != null)
                      {
                          if (!recharge_request_battery && crntbatteryblock.ChargeMode != ChargeMode.Auto && !autoChargeMode && connectorActual.Status == MyShipConnectorStatus.Connected || connectorActual.Status != MyShipConnectorStatus.Connected && crntbatteryblock.ChargeMode != ChargeMode.Auto)
                          {
                              crntbatteryblock.ChargeMode = ChargeMode.Auto;
                          }
                      } */
                }
            }

            // Calculate percentage only once, after totals are gathered
            percent_battery_power = (ttl_mPWR > 0) ? (ttl_sPWR / ttl_mPWR) * 100f : 0f;

            // Update charge states
            is_full_charge = (percent_battery_power >= bat_CHGhi);  // Use >= to catch edge cases
            is_low_charge = (percent_battery_power <= bat_CHGlow);

            // Handle recharge request
            if (!is_low_charge && recharge_request_battery && is_full_charge)
            {
                recharge_request_battery = false;
            }

            #endregion

        }

        public void fuel_check()
        {

            #region fuel_check
            if (hydrogen_tank_tag.Count <= 0 || hydrogen_tank_tag[0] == null)
            {
            }
            ttl_GASs = 0;
            ttl_sGAS = 0;
            ttl_mGAS = 0;
            ttl_GASm = 0;
            pcnt_gas_tank = 0.0;
            if (hydrogen_tank_tag.Count > 0)
            {
                for (int i = 0; i < hydrogen_tank_tag.Count; i++)
                {
                    if (hydrogen_tank_tag[i] != null)
                    {
                        if (hydrogen_tank_tag[i].IsFunctional)
                        {
                            crnthyrdogentank = hydrogen_tank_tag[i];
                            ttl_GASs = crnthyrdogentank.FilledRatio * 100.0f;
                            ttl_sGAS = ttl_sGAS + ttl_GASs;
                            ttl_mGAS = 100.0f;
                            ttl_GASm = ttl_GASm + ttl_mGAS;
                            pcnt_gas_tank = (ttl_sGAS / ttl_GASm) * 100.0f;
                        }
                    }
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
            if ((!is_low_tank && recharge_request_tank && is_full_tank && !ignore_Htank) || (ignore_Htank))
            {
                recharge_request_tank = false;
            }
            if (!recharge_request_tank && !ignore_Htank)
            {
                if (!tankAutochargeSet)
                {
                    tankupdate(false);
                    tankAutochargeSet = true;
                    tankRechargeModeSet = false;
                }
            }
            #endregion

        }

        public void recharge_state_check()
        {

            #region recharge_request_check
            if (recharge_request_battery || recharge_request_tank)
            {
                recharge_request = true;
            }
            else
            {
                recharge_request = false;
            }
            #endregion

        }

        public void check_comms_channels()
        {

            #region check_comms_channels
            if (listn.HasPendingMessage)
            {
                new_msg = listn.AcceptMessage();
                dat_in = new_msg.Data.ToString();
                _recievedMessage = dat_in;
            }
            if (listn_recall.HasPendingMessage)
            {
                new_msg_2 = listn_recall.AcceptMessage();
                dat_in2 = new_msg_2.Data.ToString();
            }
            if (listn_png.HasPendingMessage)
            {
                new_msg_3 = listn_png.AcceptMessage();
                pingedMessageDataIn = new_msg_3.Data.ToString();
            }
            if (listn_recall_drone.HasPendingMessage)
            {
                new_msg_4 = listn_recall_drone.AcceptMessage();
                dat_in4 = new_msg_4.Data.ToString();
            }

            ProcessMessages();

            if (dat_in != null)
            {
                if (_recievedMessage != jobdata)
                {
                    //FetchConfigData(Me.CustomData);
                    StoreRawInput(dat_in, Me, gmdscategory, jobinfo);
                    _updatedJob = true;
                }
                //Me.CustomData = dat_in;
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
            if (pingedMessageDataIn != null)
            {
                if (pingedMessageDataIn.Contains(pingChannelTag))
                {
                    pinged = true;
                }
                else
                {
                    pinged = false;
                }
            }
            #endregion

        }

        private void ProcessMessages()
        {
            //manage recieved communications
            if (antenna_actual != null && antenna_tag[0] != null)
            {
                if (listensync.HasPendingMessage)
                {
                    MyIGCMessage droneMessageNew = listensync.AcceptMessage();
                    syncMessagesBuffer.Add(droneMessageNew);
                }
                //process drone message list here
                if (syncMessagesBuffer.Count > 0)
                {
                    syncMessageReceived = true;
                }
                else
                {
                    syncMessageReceived = false;
                }

                if (syncMessageReceived)
                {
                    //pull first message in the list if valid
                    syncDataInput = syncMessagesBuffer[0].Data.ToString();
                    ProcessDroneMessageData(syncDataInput);
                    if (secondary_tag_changed)
                    {
                        blockRenamer();
                        secondary_tag_changed = false;
                    }
                    if (syncMessagesBuffer.Count > 0 && syncMessageReceived)
                    {
                        syncMessagesBuffer.RemoveAt(0);
                    }

                }

            }
        }

        public void ProcessDroneMessageData(string input)
        {
            secondary_tag_changed = false;

            if (secondary_tag != input)
            {
                secondary_tag = input;
                secondary_tag_changed = true;
            }
            else
            {
                return;
            }

        }

        public void custom_data_command_presence_check(string input)
        {
            if (!string.IsNullOrEmpty(input) && !string.IsNullOrWhiteSpace(input) && input != fail_data)
            {
                dataValid = true;
            }
            else dataValid = false;

            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input) || input == fail_data)
            {
                dataInvalid = true;
                if (input != fail_data)
                {
                    input = fail_data;
                }
            }
            else dataInvalid = false;
        }

        public void command_poll()
        {

            #region command_read
            if (dataValid || dataInvalid)
            {
                if (custom_data_read == 1)
                {
                    cmd_rqold = commandRequest;
                    custom_data_read = 0;
                    //droneStatus = 25;
                }
                if (custom_data_read == 0)
                {
                    if (_updatedJob)
                    {
                        GetCustomDataCommand(Me.CustomData.ToString(), Me);
                        _updatedJob = false;
                    }
                    custom_data_read = 1;
                    if (commandRequest != cmd_rqold)
                    {
                        commandChanged = true;
                    }
                    if (commandRequest == cmd_rqold)
                    {
                        commandChanged = false;
                    }
                    //droneStatus = 24;
                }
            }
            #endregion

        }

        public void drone_operating_state_mng()
        {

            #region drone_command_state_processing
            if (dataInvalid && !wasMining)
            {
                commandRequest = 0;
                droneStatusOutput = "Idle";
            }
            if ((commandRequest == 0) || (commandChanged && mode_set && commandRequest != 7))
            {
                stopState = true;
                cmd_read_ack = 0;              
            }
            else stopState = false;
            if (commandRequest == 0 && wasMining)
            {
                wasMining = false;
                targetDepthAchieved = false;
            }
            if (commandRequest == 0 && targetDepthAchieved)
            {
                targetDepthAchieved = false;
            }
            if (commandRequest >= 1 && commandRequest <= 4)
            {
                navState = true;
            }
            else navState = false;
            if (commandRequest == 5 && (!requestExit))
            {
                mineState = true;
            }
            else mineState = false;
            if (commandRequest == 6)
            {
                dockState = true;

            }
            else
            {
                dockState = false;
            }

            if (commandRequest == 7 && (!recall || !recharge_request))
            {
                undockState = true;
            }
            else undockState = false;
            if ((commandRequest == 8 && tunnelSequenceFinished) || (commandRequest == 0 && connectorActual.IsConnected && tunnelSequenceFinished && !undockState && !cargoFullAchieved && cargoIsEmpty && !recharge_request))
            {
                tunnelSequenceFinished = false;
                droneStatusOutput = "Resetting";
            }
            #endregion
            if(commandRequest == 0 && droneStatusOutput == "Docking")
            {
                droneStatusOutput = "Idle";
            }
        }

        public void connected_battery_recharge_check(bool dockingReady)
        {
            if (connectorActual != null)
            {
                if (connectorActual.IsConnected && dockingReady)
                {
                    dockingReady = false;
                }
            }
            #region connected_battery_recharge_check
            if (connectorActual != null)
            {
                if ((connectorActual.Status == MyShipConnectorStatus.Connected && ((autoChargeMode && !undockState)) || (!autoChargeMode && !undockState && recharge_request_battery)))
                {
                    if (!batteryRechargeModeSet)
                    {
                        batteryupdate(ChargeMode.Recharge);
                        batteryRechargeModeSet = true;
                        batteryAutochargeSet = false;
                    }

                }
            }
            if ((!autoChargeMode && !recharge_request_battery) || (!connectorActual.IsConnected || undockState))
            {
                if (!batteryAutochargeSet)
                {
                    batteryupdate(ChargeMode.Auto);
                    batteryRechargeModeSet = false;
                    batteryAutochargeSet = true;
                }
            }
            #endregion

        }

        public void DockingStateCheck()
        {
            int startInstructions = Runtime.CurrentInstructionCount;
            if (!dockState)
            {
                //early exit if not in dock state
                return;
            }
            if (connectorActual == null)
            {
                sbtext.AppendLine("Connector missing - exiting");
                return;
            }
            if (undockLightActual == null)
            {
                sbtext.AppendLine("Undock light missing - exiting");
                return;
            }
            if (dockLightActual == null)
            {
                sbtext.AppendLine("Docklight missing - exiting");
                return;
            }
            if (collisionAvoidLightActual == null)
            {
                sbtext.AppendLine("Collision avoidance light missing - exiting");
                return;
            }
            if (collisionSenseEnabled && sensorActual == null)
            {
                sbtext.AppendLine("Collision sensor missing missing - exiting");
                return;
            }
            if (dockState && !connectorActual.IsConnected && dockingStage == 0)
            {
                droneStatusOutput = "Docking init";
                dockingReady = false;
                if (!undockLightActual.Enabled && !connectorActual.IsConnected && connectorActual.Status != MyShipConnectorStatus.Connectable || !undockLightActual.Enabled)
                {
                    undockLightActual.Enabled = true;
                }
                if (!isDocking || !isUndocking)
                {
                    reset_ai();
                }
                dockingStage = 1;
                mainNavSequence = 0;
                collisionAvoidLightActual.Enabled = true;
                if (collisionSenseEnabled)
                {
                    if (!sensorActual.Enabled) { sensorActual.Enabled = true; }
                }
                if (dockLightActual.Enabled)
                {
                    dockLightActual.Enabled = false;
                }
            }
            if (connectorActual != null && connectorActual.Status == MyShipConnectorStatus.Connected)
            {
                if (is_low_charge && !recharge_request_battery)
                {
                    recharge_request_battery = true;
                }
                if (is_low_tank && !recharge_request_tank && !ignore_Htank)
                {
                    recharge_request_tank = true;
                }
                if (ai_task_dock_actual != null && ai_task_undock_actual != null && ai_move_actual != null)
                {
                    if (ai_task_dock_actual.GetValue<bool>(p1) && (stopState || dockState))
                    {
                        ai_task_dock_actual.GetActionWithName(poff).Apply(ai_task_dock_actual);

                        if (ai_task_undock_actual.GetValue<bool>(p1) && (stopState || dockState))
                        {
                            ai_task_undock_actual.GetActionWithName(poff).Apply(ai_task_undock_actual);
                        }
                        if (ai_move_actual.GetValue<bool>(abr) && (stopState || dockState))
                        {
                            ai_move_actual.GetActionWithName(ab0).Apply(ai_move_actual);
                        }
                        if (ai_task_dock_actual.GetValue<bool>(abr) && (stopState || dockState))
                        {
                            ai_task_dock_actual.GetActionWithName(ab0).Apply(ai_task_dock_actual);
                        }
                        if (ai_task_undock_actual.GetValue<bool>(abr) && (stopState || dockState))
                        {
                            ai_task_undock_actual.GetActionWithName(ab0).Apply(ai_task_undock_actual);
                        }
                    }

                }
            }
        }

        public void undock_management()
        {
            if (isUndocked && !switchedThrustersOn && thrustGroupPresent)
            {
                Thruster_Management(true);
                switchedThrustersOn = true;
                switchedThrustersOff = false;
            }
            if (connectorActual != null)
            {
                if (undockState && recharge_request && connectorActual.IsConnected && (is_low_charge || (is_low_tank && !ignore_Htank)))
                {
                    undockState = false;
                }
            }

            #region undock_management
            if (connectorActual != null)
            {
                if (undockState && !recharge_request && cargoIsEmpty && !cargoFullAchieved && !targetDepthAchieved && connectorActual.IsConnected && undocking_stage == 0 && (!thrustGroupPresent || thrustGroupPresent))
                {
                    if (!isDocking || !isUndocking)
                    {
                        reset_ai();
                    }
                    undocking_start = 0;
                    droneStatusOutput = "Undocking";
                    if (dockLightActual != null)
                    {
                        dockLightActual.Enabled = false;
                    }
                    connectorActual.Enabled = false;


                    if (!thrustGroupPresent)
                    {
                        if (timerBlockTONActual != null)
                        {
                            if (!timerBlockTONActual.Enabled)
                            {
                                timerBlockTONActual.Enabled = true;
                            }
                            if (!timerBlockTONActual.IsCountingDown)
                            {
                                timerBlockTONActual.Trigger();
                            }
                        }
                    }
                    else
                    {
                        Thruster_Management(true);
                        switchedThrustersOn = true;
                        switchedThrustersOff = false;
                    }
                    if (ai_move_actual != null)
                    {
                        if (ai_move_actual.SpeedLimit != nav_speed)
                        {
                            ai_move_actual.SpeedLimit = nav_speed;
                        }
                    }
                    undocking_stage = 1;
                }
            }
            if (connectorActual != null)
            {
                if (undocking_stage == 1 && !connectorActual.IsConnected)
                {
                    reset_ai();
                    connectorActual.Enabled = false;
                    if (collisionAvoidLightActual != null)
                    {
                        collisionAvoidLightActual.Enabled = false;
                    }
                    if (undockLightActual != null)
                    {
                        undockLightActual.Enabled = false;
                    }
                    if (ai_move_actual != null)
                    {
                        ai_move_actual.PrecisionMode = true;
                        ai_move_actual.CollisionAvoidance = false;
                        if (ai_move_actual.SpeedLimit != nav_speed)
                        {
                            ai_move_actual.SpeedLimit = nav_speed;
                        }
                        if (!ai_move_actual.GetValue<bool>(abr))
                        {
                            ai_move_actual.GetActionWithName(ab1).Apply(ai_move_actual);
                        }
                    }
                    if (ai_task_undock_actual != null)
                    {
                        if (!ai_task_undock_actual.GetValue<bool>(abr))
                        {
                            ai_task_undock_actual.GetActionWithName(ab1).Apply(ai_task_undock_actual);
                        }
                        if (!ai_task_undock_actual.GetValue<bool>(p1))
                        {
                            ai_task_undock_actual.GetActionWithName(pon).Apply(ai_task_undock_actual);
                        }
                    }
                    undocking_stage = 2;
                    if (thrustGroupPresent && !switchedThrustersOn)
                    {
                        Thruster_Management(true);
                        switchedThrustersOn = true;
                        switchedThrustersOff = false;
                    }
                }
            }

            if (undocking_stage == 2 && undockLightActual.Enabled && !connectorActual.IsConnected)
            {
                if (collisionAvoidLightActual != null)
                {
                    collisionAvoidLightActual.Enabled = false;
                }
                if (collisionSenseEnabled)
                {
                    if (sensorActual != null)
                    {
                        if (sensorActual.Enabled) { sensorActual.Enabled = false; }
                    }
                }
                if (ai_move_actual != null)
                {
                    ai_move_actual.PrecisionMode = false;
                    ai_move_actual.CollisionAvoidance = true;
                    if (ai_move_actual.SpeedLimit != nav_speed)
                    {
                        ai_move_actual.SpeedLimit = nav_speed;
                    }
                }
                if (connectorActual != null)
                {
                    connectorActual.Enabled = true;
                }
                undocking_stage = 3;
            }
            if (connectorActual != null && ai_task_undock_actual != null && undockLightActual != null)
            {
                if (!connectorActual.IsConnected && !ai_task_undock_actual.GetValue<bool>(p1) && undocking_stage == 2 && !undockLightActual.Enabled)
                {
                    if (udock_conf)
                    {
                        undocking_stage = 1;
                    }
                    if (!udock_conf)
                    {
                        undockLightActual.Enabled = true;
                    }
                }
            }

            if (isUndocked && undocking_stage == 3 && undocking_start == 0)
            {
                droneStatusOutput = "Undocked";
                reset_ai();
                undocking_start = 1;
            }

            GetSpeed();

            if (undocking_stage > 0 && undocking_stage < 3 && spd <= currentSpeedNotMovingThreshold)
            {
                isUndocking = true;
                if (ai_task_undock_actual != null)
                {
                    if (!ai_task_undock_actual.GetValue<bool>(p1))
                    {
                        no_speed_undock_delay_count++;
                    }
                }
                undock_delay_time = Math.Round(((double)no_speed_undock_delay_count * (double)10 * game_tick_length) / (double)1000, 1);
            }
            if (no_speed_ready_undock && undocking_stage > 0 && undocking_stage < 3 && !connectorActual.IsConnected)
            {
                if (resetLightActual != null)
                {
                    if (!resetLightActual.Enabled)
                    {
                        resetLightActual.Enabled = true;
                    }
                }
                if (undockLightActual != null)
                {
                    if (!undockLightActual.Enabled)
                    {
                        undockLightActual.Enabled = true;
                    }
                }
                undocking_stage = 3;
            }

            if (undocking_stage > 2 && isUndocked)
            {
                isUndocking = false;
            }
            #endregion

        }

        public void dock_undock_state_check()
        {
            int startInstructions = Runtime.CurrentInstructionCount;
            #region dock_undock_state_check
            if (dockingStage > 0)
            {
                isDocking = true;
            }
            else isDocking = false;
            if (isUndocking || isDocking)
            {
                isAutopiloting = true;
            }
            else isAutopiloting = false;
            if (dockLightActual != null)
            {
                if (dockLightActual.Enabled)
                {
                    isDocked = true;
                }
                else isDocked = false;
            }
            else
            {
                isDocked = false;
            }
            if (undockLightActual != null)
            {
                if (undockLightActual.Enabled)
                {
                    isUndocked = true;
                }
                else isUndocked = false;
            }
            else
            {
                isUndocked = false;
            }

            #endregion

        }

        public void drone_diver_state_management()
        {

            #region drone_diver_state_management
            if (stopState)
            {
                mainNavSequence = 0;
                main_nav_complete = false;
                add_nav_Waypoint_mn = false;
                if (!wasMining)
                {
                    if (remoteControlActual != null)
                    {
                        remoteControlActual.SetCollisionAvoidance(false);
                        remoteControlActual.SetDockingMode(false);
                        remoteControlActual.SetAutoPilotEnabled(false);
                        remoteControlActual.ClearWaypoints();
                    }
                    miningInitialised = false;
                    add_mine_waypoint = false;
                }
                if (ai_task_dock_actual != null)
                {
                    if (ai_task_dock_actual.GetValue<bool>(p1))
                    {
                        ai_task_dock_actual.GetActionWithName(poff).Apply(ai_task_dock_actual);
                    }
                    if (ai_task_dock_actual.GetValue<bool>(abr))
                    {
                        ai_task_dock_actual.GetActionWithName(ab0).Apply(ai_task_dock_actual);
                    }
                }
                if (ai_task_undock_actual != null)
                {
                    if (ai_task_undock_actual.GetValue<bool>(p1))
                    {
                        ai_task_undock_actual.GetActionWithName(poff).Apply(ai_task_undock_actual);
                    }
                    if (ai_task_undock_actual.GetValue<bool>(abr))
                    {
                        ai_task_undock_actual.GetActionWithName(ab0).Apply(ai_task_undock_actual);
                    }
                }
                if (ai_move_actual != null)
                {
                    if (ai_move_actual.GetValue<bool>(abr))
                    {
                        ai_move_actual.GetActionWithName(ab0).Apply(ai_move_actual);
                    }
                }

                if (!requestExit)
                {
                    if (!dockingReady)
                    {
                        miningStage = 0;
                    }
                    mineState = false;
                }
                navState = false;
                dockState = false;
                droneStatusOutput = "Idle";

                undocking_stage = 0;
                droneStatus = 0;
               
            }
            //reset exit request on stop state
            if (miningStage == 0 && stopState && requestExit)
            {
                requestExit = false;
            }

            if (mineState && !wasMining && !dockingReady)
            {
                wasMining = true;
            }

            if (reset_mining && wasMining)
            {
                wasMining = false;
                reset_mining = false;
                if (!dockingReady)
                {
                    miningStage = 0;
                }
            }
            if (reset_mining && !wasMining)
            {
                reset_mining = false;
            }
            if (!mineState && !wasMining && miningStage > 0)
            {
                if (!dockingReady)
                {
                    miningStage = 0;
                }
            }
            if (mineState || navState)
            {
                mode_set = true;
            }
            else mode_set = false;


            if (dataInvalid && targetDepthAchieved && !requestExit && wasMining && custom_data_read == 1 && isUndocked)
            {
                requestExit = true;
            }
            if (targetDepthAchieved || cargoFullAchieved || recharge_request || recall)
            {
                force_request_dock = true;
            }
            else force_request_dock = false;
            #endregion

        }

        public void check_for_planetary_gravity_presence()
        {

            #region check_for_planetary_gravity_presence
            if (remoteControlActual != null)
            {
                gravity = remoteControlActual.GetNaturalGravity();
            }
            if (targetAlignmentValid)
            {
                crnt_tgt_align = alignmentTargetNew;
            }
            if (!targetAlignmentValid)
            {
                crnt_tgt_align = gravity;
            }
            #endregion

        }
        public void gravity_alignment_mng()
        {
            if (gravity == Vector3D.Zero)
            {
                gravityPresent = false;
            }
            else
            {
                gravityPresent = true;
            }
        }
        public void check_ai_gravity_setting()
        {
            #region check_ai_gravity_setting
            if (gravityPresent)
            {
                if (ai_move_actual != null)
                {
                    if (!ai_move_actual.AlignToPGravity)
                    {
                        ai_move_actual.AlignToPGravity = true;
                    }
                }
            }
            if (!gravityPresent)
            {
                if (ai_move_actual != null)
                {
                    if (ai_move_actual.AlignToPGravity)
                    {
                        ai_move_actual.AlignToPGravity = false;
                    }
                }
            }
            #endregion
        }


        public void drone_alignment_management()
        {
            Vector3D NavTemp;
            #region drone_alignment_management
            if (isDocked || !isUndocked && !isDocked || isUndocking || isDocking || stopState)
            {
                can_gyroOVR = false;
            }
            else if (navState || (mineState && !(isDocking || isUndocking)))
            {
                can_gyroOVR = true;
            }
            if (miningStage >= 6 && miningStage <= 10)
            {
                can_gyroOVR = true;
            }
            if(navState || mineState)
            {
                NavTemp = GetNavAngles(crnt_tgt_align) * GyrMlt;
            }
            else
            {
                NavTemp = Vector3D.Zero;
            }

                SetGyroOverride(can_gyroOVR, NavTemp, isDocked);

            double YawMon = NavTemp.GetDim(0);
            double PitchMon = NavTemp.GetDim(1);
            double RollMon = NavTemp.GetDim(2);

            if (YawMon > nav_inst_thr && !isDocked || YawMon < -nav_inst_thr && !isDocked)
            {
                droneStatus = 23;
                yawinst = true;
            }
            else yawinst = false;

            if (PitchMon > nav_inst_thr && !isDocked || PitchMon < -nav_inst_thr && !isDocked)
            {
                pitchinst = true;
                droneStatus = 23;
            }
            else pitchinst = false;
            if (RollMon > nav_inst_thr && !isDocked || RollMon < -nav_inst_thr && !isDocked)
            {
                rollinst = true;
                droneStatus = 23;
            }
            else rollinst = false;

            if (mainNavSequence > 0 && mainNavSequence < 4 && targetAlignmentValid)
            {
                nav_act = true;
            }
            else
            {
                nav_act = false;
            }
            if (yawinst && !nav_act && !isDocked  || pitchinst && !nav_act && !isDocked  || rollinst && !nav_act && !isDocked  || resetLightActual.Enabled && !isDocking && !isDocked)
            {
                navinst = true;
                droneStatus = 23;
            }
            else
            {
                navinst = false;
            }

            #endregion

        }

        public void navigation_management()
        {

            int startInstructions = Runtime.CurrentInstructionCount;
            if (!navState)
            {
                return;
            }
            if (remoteControlActual == null) { sbtext.AppendLine("Error: Remote control is null in navigation_management"); return; }
            #region navigation_management
            remote_control_position_update();
            if (connectorActual != null)
            {
                if ((!add_nav_Waypoint_mn && mainNavSequence == 0 && custom_data_read == 1 && navState && !(connectorActual.IsConnected || connectorActual.Status == MyShipConnectorStatus.Connectable) && isUndocked) && !main_nav_complete)
                {
                    remoteControlActual.ClearWaypoints();
                    //main_nav_complete = false;
                    mainNavSequence = 1;
                    droneStatus = 1;
                    droneStatusOutput = "mine nav gps--";
                    if (!undockLightActual.Enabled)
                    {
                        undockLightActual.Enabled = true;
                    }
                    if (dockLightActual.Enabled)
                    {
                        dockLightActual.Enabled = false;
                    }
                }
                if ((!add_nav_Waypoint_mn && mainNavSequence == 1 && custom_data_read == 1 && navState && !(connectorActual.IsConnected || connectorActual.Status == MyShipConnectorStatus.Connectable) && isUndocked))
                {
                    remoteControlActual.ClearWaypoints();
                    add_nav_Waypoint_mn = true;
                    main_nav_complete = false;
                    mainNavSequence = 2;
                    remoteControlActual.AddWaypoint(main_gps_coords, "mine nav gps");
                    droneStatus = 1;
                    droneStatusOutput = "Nav";
                    if (!undockLightActual.Enabled)
                    {
                        undockLightActual.Enabled = true;
                    }
                    if (dockLightActual.Enabled)
                    {
                        dockLightActual.Enabled = false;
                    }
                }
            }
            else
            {
                setupIsComplete = false;
                return;
            }
            if (mainNavSequence == 2 && rc_xyz != remoteControlActual.CurrentWaypoint.Coords && navState && isUndocked && !main_nav_complete && add_nav_Waypoint_mn)
            {
                mainNavSequence = 3;
                remoteControlActual.SpeedLimit = nav_speed;

                if (commandRequest == 1)
                {
                    remoteControlActual.SetCollisionAvoidance(true);
                    remoteControlActual.SetDockingMode(false);
                    remoteControlActual.SetAutoPilotEnabled(!navinst);
                    droneStatus = 1;
                    reset_ai();
                    if (resetLightActual != null)
                    {
                        if (resetLightActual.Enabled)
                        {
                            resetLightActual.Enabled = false;
                        }
                    }

                }
                if (commandRequest == 2)
                {
                    remoteControlActual.SetCollisionAvoidance(false);
                    remoteControlActual.SetDockingMode(false);
                    remoteControlActual.SetAutoPilotEnabled(!navinst);
                    droneStatus = 2;
                    reset_ai();
                    if (resetLightActual != null)
                    {
                        if (resetLightActual.Enabled)
                        {
                            resetLightActual.Enabled = false;
                        }
                    }
                }
                if (commandRequest == 3)
                {
                    remoteControlActual.SetCollisionAvoidance(false);
                    remoteControlActual.SetDockingMode(true);
                    remoteControlActual.SetAutoPilotEnabled(!navinst);
                    droneStatus = 3;
                }
                if (commandRequest == 4)
                {
                    remoteControlActual.SetCollisionAvoidance(true);
                    remoteControlActual.SetDockingMode(false);
                    remoteControlActual.SetAutoPilotEnabled(!navinst);
                    droneStatus = 4;
                    if (collisionSenseEnabled)
                    {
                        if (sensorActual != null)
                        {
                            if (!sensorActual.Enabled)
                            {
                                if (!sensorActual.Enabled) { sensorActual.Enabled = true; }
                            }
                        }
                    }
                }
                droneStatusOutput = "Nav";
                if (undockLightActual != null)
                {
                    if (!undockLightActual.Enabled)
                    {
                        undockLightActual.Enabled = true;
                    }
                }
                if (dockLightActual != null)
                {
                    if (dockLightActual.Enabled)
                    {
                        dockLightActual.Enabled = false;
                    }
                }
            }

            if ((mainNavSequence == 3 && navinst && commandRequest == 1) || (mainNavSequence == 3 && navinst && commandRequest == 4))
            {
                remoteControlActual.ClearWaypoints();
                mainNavSequence = 1;
                add_nav_Waypoint_mn = false;
                reset_ai();
                if (resetLightActual != null)
                {
                    if (resetLightActual.Enabled)
                    {
                        resetLightActual.Enabled = false;
                    }
                }
            }

            //            GetSpeed();

            if ((spd <= currentSpeedNotMovingThreshold && no_speed_count_navigation_reset_delay_count < no_speed_navigation_delay_limit && mainNavSequence == 3 && !resetLightActual.Enabled && !navinst && commandRequest == 4) || (spd <= currentSpeedNotMovingThreshold && no_speed_count_navigation_reset_delay_count < no_speed_navigation_delay_limit && mainNavSequence == 3 && !resetLightActual.Enabled && !navinst && commandRequest == 1))
            {
                no_speed_count_navigation_reset_delay_count++;
                navigation_reset_delay_time = Math.Round(((double)no_speed_count_navigation_reset_delay_count * (double)10 * game_tick_length) / (double)1000, 1);

            }
            if ((mainNavSequence == 3 && resetLightActual.Enabled && commandRequest == 1) || (mainNavSequence == 3 && resetLightActual.Enabled && commandRequest == 4 || navigation_reset_delay))
            {
                remoteControlActual.ClearWaypoints();
                mainNavSequence = 0;
                add_nav_Waypoint_mn = false;
                reset_ai();
                if (resetLightActual != null)
                {
                    if (resetLightActual.Enabled)
                    {
                        resetLightActual.Enabled = false;
                    }
                }
                navigation_reset_delay = false;
                no_speed_count_navigation_reset_delay_count = 0;
                navigation_reset_delay_time = Math.Round(((double)no_speed_count_navigation_reset_delay_count * (double)10 * game_tick_length) / (double)1000, 1);
            }

            double rc_cw_x = main_gps_coords.X;
            double rc_cw_y = main_gps_coords.Y;
            double rc_cw_z = main_gps_coords.Z;
            if (mainNavSequence == 3 && rc_xyz != remoteControlActual.CurrentWaypoint.Coords && navState && isUndocked && !main_nav_complete && add_nav_Waypoint_mn && !remoteControlActual.IsAutoPilotEnabled && !navinst)
            {
                remoteControlActual.SpeedLimit = nav_speed;
                if (commandRequest == 1)
                {
                    remoteControlActual.SetCollisionAvoidance(true);
                    remoteControlActual.SetDockingMode(false);
                    remoteControlActual.SetAutoPilotEnabled(!navinst);
                    droneStatus = 1;
                    reset_ai();
                    if (resetLightActual != null)
                    {
                        if (resetLightActual.Enabled)
                        {
                            resetLightActual.Enabled = false;
                        }
                    }
                }
                if (commandRequest == 2)
                {
                    remoteControlActual.SetCollisionAvoidance(false);
                    remoteControlActual.SetDockingMode(false);
                    remoteControlActual.SetAutoPilotEnabled(!navinst);
                    droneStatus = 2;
                    reset_ai();
                    if (resetLightActual.Enabled)
                    {
                        resetLightActual.Enabled = false;
                    }
                }
                if (commandRequest == 3)
                {
                    remoteControlActual.SetCollisionAvoidance(false);
                    remoteControlActual.SetDockingMode(true);
                    remoteControlActual.SetAutoPilotEnabled(!navinst);
                    droneStatus = 3;
                }
                if (commandRequest == 4)
                {
                    remoteControlActual.SetCollisionAvoidance(true);
                    remoteControlActual.SetDockingMode(true);
                    remoteControlActual.SetAutoPilotEnabled(!navinst);
                    droneStatus = 4;
                    if (collisionSenseEnabled)
                    {
                        if (!sensorActual.Enabled)
                        {
                            if (!sensorActual.Enabled) { sensorActual.Enabled = true; }
                        }
                    }

                }
                droneStatusOutput = "Nav";
                if (undockLightActual != null)
                {
                    if (!undockLightActual.Enabled)
                    {
                        undockLightActual.Enabled = true;
                    }
                }
                if (dockLightActual != null)
                {
                    if (dockLightActual.Enabled)
                    {
                        dockLightActual.Enabled = false;
                    }
                }
            }
            if (mainNavSequence == 3 && rc_xyz.X >= rc_cw_x - nav_prec && rc_xyz.X <= rc_cw_x + nav_prec && rc_xyz.Y >= rc_cw_y - nav_prec && rc_xyz.Y <= rc_cw_y + nav_prec && rc_xyz.Z >= rc_cw_z - nav_prec && rc_xyz.Z <= rc_cw_z + nav_prec && navState && isUndocked && !main_nav_complete && add_nav_Waypoint_mn)
            {
                mainNavSequence = 4;
                main_nav_complete = true;
                add_nav_Waypoint_mn = false;
                remoteControlActual.SetCollisionAvoidance(true);
                remoteControlActual.SetDockingMode(false);
                remoteControlActual.SetAutoPilotEnabled(false);
                remoteControlActual.ClearWaypoints();
                droneStatus = 5;
                droneStatusOutput = "Nav End";
                if (undockLightActual != null)
                {
                    if (!undockLightActual.Enabled)
                    {
                        undockLightActual.Enabled = true;
                    }
                }
                if (dockLightActual != null)
                {
                    if (dockLightActual.Enabled)
                    {
                        dockLightActual.Enabled = false;
                    }
                }
                mainNavSequence = 0;
                
            }
            remoteControlActual.GetWaypointInfo(waypoints);
            if (mainNavSequence == 3 && add_nav_Waypoint_mn && waypoints.Count <= 0 && !main_nav_complete && isUndocked && navState)
            {
                mainNavSequence = 4;
                main_nav_complete = true;
                add_nav_Waypoint_mn = false;
                remoteControlActual.SetCollisionAvoidance(true);
                remoteControlActual.SetDockingMode(false);
                remoteControlActual.SetAutoPilotEnabled(false);
                remoteControlActual.ClearWaypoints();
                droneStatus = 5;
                droneStatusOutput = "Nav End";
                if (!undockLightActual.Enabled)
                {
                    undockLightActual.Enabled = true;
                }
                if (dockLightActual != null)
                {
                    if (dockLightActual.Enabled)
                    {
                        dockLightActual.Enabled = false;
                    }
                }
                mainNavSequence = 0;
                
            }

            if ((mainNavSequence > 0 && recharge_request) || (mainNavSequence > 0 && force_request_dock) || (mainNavSequence > 0 && (connectorActual.IsConnected || connectorActual.Status == MyShipConnectorStatus.Connectable)))
            {
                mainNavSequence = 0;
                main_nav_complete = true;
                add_nav_Waypoint_mn = false;
                remoteControlActual.SetCollisionAvoidance(true);
                remoteControlActual.SetDockingMode(false);
                remoteControlActual.SetAutoPilotEnabled(false);
                remoteControlActual.ClearWaypoints();
                exitWaypointSet = false;
                exitSequenceComplete = false;
                droneStatus = 21;
                if (wasMining)
                {
                    reset_mining = true;
                }
                requestExit = false;
                wasMining = false;
                targetDepthAchieved = false;
                if (!isDocking || !isUndocking)
                {
                    reset_ai();
                    if (resetLightActual != null)
                    {
                        if (resetLightActual.Enabled)
                        {
                            resetLightActual.Enabled = false;
                        }
                    }
                }
                dockingStage = 1;
                if (!collisionAvoidLightActual.Enabled)
                {
                    collisionAvoidLightActual.Enabled = true;
                }
                if (collisionSenseEnabled)
                {
                    if (sensorActual != null)
                    {
                        if (!sensorActual.Enabled) { sensorActual.Enabled = true; }
                    }
                }
                if (undockLightActual != null)
                {
                    if (!undockLightActual.Enabled)
                    {
                        undockLightActual.Enabled = true;
                    }
                }
                if (dockLightActual != null)
                {
                    if (dockLightActual.Enabled)
                    {
                        dockLightActual.Enabled = false;
                    }
                }
                droneStatusOutput = "RTB";
            }
            #endregion

        }

        public void mining_management(bool autoDock)
        {
            if (remoteControlActual == null) { sbtext.AppendLine("Error: Remote control is null in mining_management"); return; }

            #region mining_management
            // *** Mining sequence ***
            remote_control_position_update();
            //initialise mining position
            if (!miningInitialised && mineState && custom_data_read == 1 && miningStage == 0 && !isAutopiloting && isUndocked)
            {
                droneStatusOutput = "Calculating mineshaft";
                mine_coords_adjusted = false;
                tgt_drill_start.X = main_gps_coords.X;
                tgt_drill_start.Z = main_gps_coords.Z;
                tgt_drill_start.Y = main_gps_coords.Y;

                if (targetAlignmentValid)
                {
                    directionb = Vector3D.Normalize(new Vector3D(-(main_gps_coords - crnt_tgt_align)));
                }
                else if (!targetAlignmentValid)
                {
                    directionb = Vector3D.Normalize(new Vector3D(gravity));
                }

                Vector3D targetpositiont = directionb * drillSetLength;
                tgt_drill_end.Y = Math.Round(tgt_drill_start.Y + targetpositiont.Y, 2);
                tgt_drill_end.X = Math.Round(tgt_drill_start.X + targetpositiont.X, 2);
                tgt_drill_end.Z = Math.Round(tgt_drill_start.Z + targetpositiont.Z, 2);
                droneStatus = 6;
                miningInitialised = true;
            }
            // Check if depth is achieved
            double distance_to_target = Vector3D.Distance(rc_xyz, tgt_drill_end);

            // Check overshoot
            Vector3D normalizedVector = directionb;
            Vector3D displacement = rc_xyz - tgt_drill_start;

            double projectionDistance = Vector3D.Dot(displacement, normalizedVector);
            bool hasOvershot = projectionDistance > drillSetLength + termnationPrecision;
            if (!targetDepthAchieved &&
                rc_xyz.X >= tgt_drill_end.X - termnationPrecision && rc_xyz.X <= tgt_drill_end.X + termnationPrecision &&
                rc_xyz.Y >= tgt_drill_end.Y - termnationPrecision && rc_xyz.Y <= tgt_drill_end.Y + termnationPrecision &&
                rc_xyz.Z >= tgt_drill_end.Z - termnationPrecision && rc_xyz.Z <= tgt_drill_end.Z + termnationPrecision &&
                mineState && miningInitialised && isUndocked && miningStage > 0)
            {
                targetDepthAchieved = true;
            }
            else if (!targetDepthAchieved && hasOvershot && mineState && miningInitialised && isUndocked)
            {
                targetDepthAchieved = true;
                droneStatus = 26;
                sbtext.AppendLine("Overshoot detected");
            }
            // Check depth achieved
            else if (!targetDepthAchieved && distance_to_target <= termnationPrecision && mineState && miningInitialised && isUndocked)
            {
                targetDepthAchieved = true;
                sbtext.AppendLine("Target depth achieved");
            }
            else
            {
                targetDepthAchieved = false;
            }
            //if depth is achieved set tunnel bore sequence finsished flag to true
            if (targetDepthAchieved && !tunnelSequenceFinished)
            {
                tunnelSequenceFinished = true;
            }
            //check if cargo is full and mining has been initialised
            if (cargoIsFull && mineState && miningInitialised)
            {
                cargoFullAchieved = true;
            }
            //check if battery is low to request recharge
            if ((is_low_charge && mineState && miningInitialised && !recharge_request_battery) || (is_low_charge && connectorActual.IsConnected && !recharge_request_battery) || (is_low_charge && !connectorActual.IsConnected && !recharge_request_battery && mainNavSequence > 0 && !mineState && isUndocked))
            {
                recharge_request_battery = true;
            }
            //check if tank is low to request gas recharge if tanks is not ignored
            if (((is_low_tank && mineState && miningInitialised && !recharge_request_tank && !ignore_Htank) || (is_low_tank && connectorActual.IsConnected && !recharge_request_tank && !ignore_Htank)) || (is_low_tank && !connectorActual.IsConnected && !recharge_request_tank && !ignore_Htank && mainNavSequence > 0 && !mineState && isUndocked))
            {
                recharge_request_tank = true;
            }
            //if all pre checks are ok, drone is undocked and ready - initiate mining sequence           
            if (!targetDepthAchieved && miningStage == 0 && mineState && miningInitialised && !isAutopiloting && !connectorActual.IsConnected && isUndocked)
            {
                miningStage = 1;
                droneStatus = 7;
                droneStatusOutput = "Initiating mining";
                reset_ai(); //reset ai blocks to ensure no AI move block interference with mining sequence
                if (resetLightActual.Enabled)
                {
                    resetLightActual.Enabled = false;
                }
            }
            //mining sequence

            if (miningStage == 1 && !targetDepthAchieved && mineState && miningInitialised && !isAutopiloting && isUndocked && !mine_coords_adjusted) // scan coordinate position to ground
            {
                miningStage = 2;
                mine_coords_adjusted = true;
                InitializeMining_Coordinates();
                if(!navinst && !initiateDrills)
                {
                    initiateDrills = true;
                }

                StDrlOnOff(initiateDrills, cnvyrsON, terrainclearEnable);
                droneStatus = 8;
                droneStatusOutput = "Mining";
            }
            if (miningStage == 1 && !targetDepthAchieved && mineState && miningInitialised && !isAutopiloting && isUndocked && mine_coords_adjusted) // scan coordinate position to ground
            {

                StDrlOnOff(false, cnvyrsON, terrainclearEnable);
                if (initiateDrills)
                {
                    initiateDrills = false;
                }
                droneStatus = 8;
                droneStatusOutput = "Initiating RTB"; //tDA Bug - TDA was being removed early
            }
            if (miningStage == 2 && !add_mine_waypoint && !targetDepthAchieved && mineState && miningInitialised && !isAutopiloting && isUndocked)
            {

                miningStage = 3;
                add_mine_waypoint = true;
                if (remoteControlActual.SpeedLimit != drill_speed)
                {
                    remoteControlActual.SpeedLimit = drill_speed; //initialise mining drill speed
                }
                Calculate_miningCoords();
                //remoteControlActual.ClearWaypoints(); //clear any existing waypoints to safely add new mining location
                remoteControlActual.AddWaypoint(mining_gps_coords, "mineloc");
                droneStatus = 9;
                droneStatusOutput = "Mining+";
            }
            if (miningStage == 3 && add_mine_waypoint && !targetDepthAchieved && mineState && miningInitialised && !isAutopiloting && isUndocked)
            {
                miningStage = 4;

                remoteControlActual.SetCollisionAvoidance(false);
                remoteControlActual.SetDockingMode(true);
                remoteControlActual.SetAutoPilotEnabled(!navinst);
                droneStatus = 10;
                droneStatusOutput = "Mining++";
            }
            if (miningStage == 3 && add_mine_waypoint && !targetDepthAchieved && mineState && miningInitialised && !isAutopiloting && isUndocked && !remoteControlActual.IsAutoPilotEnabled)
            {
                miningStage = 2;
                add_mine_waypoint = false;
                droneStatus = 10;
                droneStatusOutput = "Mining++";
            }

            //variables to shorten if statement chars
            double rc_cmw_x = mining_gps_coords.X;
            double rc_cmw_y = mining_gps_coords.Y;
            double rc_cmw_z = mining_gps_coords.Z;
            if (miningStage == 4 && !mining_nav_complete && add_mine_waypoint && !targetDepthAchieved && remoteControlActual.CurrentWaypoint.Name == null && mineState && miningInitialised && !requestExit && !isAutopiloting && isUndocked)
            {
                miningStage = 1;
                remoteControlActual.SetAutoPilotEnabled(!navinst);
                add_mine_waypoint = false;
                mine_coords_adjusted = false;
                droneStatus = 11;
                droneStatusOutput = "Mining++-";
            }
            if (miningStage == 4 && !mining_nav_complete && add_mine_waypoint && !targetDepthAchieved && !remoteControlActual.IsAutoPilotEnabled && mineState && miningInitialised && !requestExit && !isAutopiloting && isUndocked)
            {
                miningStage = 2;
                remoteControlActual.SetAutoPilotEnabled(!navinst);
                add_mine_waypoint = false;
                droneStatus = 11;
                droneStatusOutput = "Mining+++";
            }

            if (miningStage == 4 && !mining_nav_complete && add_mine_waypoint && !targetDepthAchieved && rc_xyz.X >= rc_cmw_x - mine_prec && rc_xyz.X <= rc_cmw_x + mine_prec && rc_xyz.Y >= rc_cmw_y - mine_prec && rc_xyz.Y <= rc_cmw_y + mine_prec && rc_xyz.Z >= rc_cmw_z - mine_prec && rc_xyz.Z <= rc_cmw_z + mine_prec && mineState && miningInitialised && !requestExit && !isAutopiloting && isUndocked)
            {
                miningStage = 5;
                mining_nav_complete = true;
                remoteControlActual.SetCollisionAvoidance(false);
                remoteControlActual.SetDockingMode(true);
                remoteControlActual.SetAutoPilotEnabled(false);
                droneStatus = 12;
                droneStatusOutput = "Mining++++";
            }

            if (miningStage == 4 && mining_nav_complete && add_mine_waypoint && !targetDepthAchieved && !remoteControlActual.IsAutoPilotEnabled && mineState && miningInitialised && !requestExit && !isAutopiloting && isUndocked)
            {
                miningStage = 5;
                remoteControlActual.SetCollisionAvoidance(false);
                remoteControlActual.SetDockingMode(true);
                remoteControlActual.SetAutoPilotEnabled(false);
                mining_nav_complete = true;
                droneStatus = 12;
                droneStatusOutput = "Mining+++";
            }
            if (miningStage == 4 && !mining_nav_complete && !targetDepthAchieved && dataInvalid && wasMining && miningInitialised && !requestExit && !isAutopiloting && isUndocked)
            {
                miningStage = 6;
                requestExit = true;
                //  Last_Coords_Term = main_gps_coords;
                exitWaypointSet = false;
                exitSequenceComplete = false;
                remoteControlActual.SpeedLimit = exit_speed;
                remoteControlActual.ClearWaypoints();
                remoteControlActual.SetCollisionAvoidance(false);
                remoteControlActual.SetDockingMode(false);
                remoteControlActual.SetAutoPilotEnabled(false);
                droneStatus = 13;
                droneStatusOutput = "Terminating mining";
            }
            if (miningStage >= 1 && miningStage <= 4 && !mining_nav_complete && !targetDepthAchieved && requestExit && wasMining && miningInitialised && remoteControlActual.CurrentWaypoint.Name != "exit shaft" && !isAutopiloting && isUndocked)
            {
                miningStage = 6;
                //  Last_Coords_Term = main_gps_coords;
                exitWaypointSet = false;
                exitSequenceComplete = false;
                remoteControlActual.SpeedLimit = exit_speed;
                remoteControlActual.SetCollisionAvoidance(false);
                remoteControlActual.SetDockingMode(false);
                remoteControlActual.SetAutoPilotEnabled(false);
                remoteControlActual.ClearWaypoints();
                droneStatus = 13;
                droneStatusOutput = "Terminating mining";
            }
            if (force_request_dock && miningStage >= 1 && miningStage <= 4 && !mining_nav_complete && dataValid && custom_data_read == 1 && wasMining && miningInitialised && !requestExit && !isAutopiloting && isUndocked)
            {

                miningStage = 6;
                requestExit = true;
                //  Last_Coords_Term = main_gps_coords;
                exitWaypointSet = false;
                exitSequenceComplete = false;
                remoteControlActual.SpeedLimit = exit_speed;
                remoteControlActual.ClearWaypoints();
                remoteControlActual.SetCollisionAvoidance(false);
                remoteControlActual.SetDockingMode(false);
                remoteControlActual.SetAutoPilotEnabled(false);
                droneStatus = 14;
                droneStatusOutput = "Terminating mining";
            }
            if (miningStage == 5 && mining_nav_complete && force_request_dock && mineState && miningInitialised && !requestExit && !isAutopiloting && isUndocked)
            {
                miningStage = 6;
                requestExit = true;
                // Last_Coords_Term = main_gps_coords;
                remoteControlActual.SpeedLimit = exit_speed;
                remoteControlActual.SetCollisionAvoidance(false);
                remoteControlActual.SetDockingMode(false);
                remoteControlActual.SetAutoPilotEnabled(false);
                remoteControlActual.ClearWaypoints();
                exitWaypointSet = false;
                exitSequenceComplete = false;
                droneStatus = 16;
                droneStatusOutput = "Terminating mining";
            }
            if (miningStage == 5 && mining_nav_complete && !targetDepthAchieved && !force_request_dock && mineState && miningInitialised && !requestExit && !isAutopiloting && isUndocked)
            {
                miningStage = 1;
                targetDepthAchieved = false;
                if (remoteControlActual.SpeedLimit != exit_speed)
                {
                    remoteControlActual.SpeedLimit = exit_speed; //initialise mining exit speed
                }
                remoteControlActual.SetCollisionAvoidance(false);
                remoteControlActual.SetDockingMode(false);
                remoteControlActual.SetAutoPilotEnabled(false);
                remoteControlActual.ClearWaypoints();
                mine_coords_adjusted = false;
                add_mine_waypoint = false;
                mining_nav_complete = false;
                droneStatus = 15;
                droneStatusOutput = "Mining";
            }
            distance_current = (remoteControlActual.GetPosition() - tgt_drill_end).Length();
            //manage converyor operation
            if ((distance_current <= drillSetLength - ignoreDistance) || connectorActual.IsConnected || sens_convOPN)
            {
                cnvyrsON = true;
            }
            else
            {
                cnvyrsON = false;
            }
            //manage terrain clearing mode
            if ((distance_current <= drillSetLength - ignoreDistance + terrainclearoffset) || connectorActual.IsConnected || sens_convOPN || terrainKeepMode)
            {
                terrainclearEnable = false;

            }
            else
            {
                terrainclearEnable = true;
            }
            if (miningStage == 6 && !exitWaypointSet && !exitSequenceComplete && wasMining && miningInitialised && requestExit && !isAutopiloting && isUndocked)
            {
                miningStage = 7;
                exitWaypointAdjusted = false;
                exitWaypointSet = true;
                exitSequenceComplete = false;
                reset_ai();
                if (resetLightActual.Enabled)
                {
                    resetLightActual.Enabled = false;
                }
                if (remoteControlActual.SpeedLimit != exit_speed)
                {
                    remoteControlActual.SpeedLimit = exit_speed; //initialise mining exit speed
                }
                if (targetAlignmentValid)
                {
                    directionc = Vector3D.Normalize(new Vector3D(-(main_gps_coords - crnt_tgt_align)));
                }
                else if (!targetAlignmentValid)
                {
                    directionc = Vector3D.Normalize(new Vector3D(gravity));
                }
                Vector3D targetpositione = directionc * drill_el;
                Vector3D targetpositione_temp = directionc * req_dist;

                tgt_drill_exit.Y = Math.Round(tgt_drill_start.Y - targetpositione.Y, 2);
                tgt_drill_exit.X = Math.Round(tgt_drill_start.X - targetpositione.X, 2);
                tgt_drill_exit.Z = Math.Round(tgt_drill_start.Z - targetpositione.Z, 2);
                droneStatus = 17;
                droneStatusOutput = "Exit path";
            }
            if (miningStage == 7 && exitWaypointSet && !exitSequenceComplete && miningInitialised && requestExit && !isAutopiloting && isUndocked)
            {
                if (!exitWaypointAdjusted)
                {
                    miningStage = 8;
                    exitWaypointAdjusted = true;
                    if (targetAlignmentValid)
                    {
                        direction = Vector3D.Normalize(new Vector3D(-(main_gps_coords - crnt_tgt_align)));
                    }
                    else if (!targetAlignmentValid)
                    {
                        direction = Vector3D.Normalize(new Vector3D(gravity));
                    }

                    Vector3D targetposition = direction * req_dist;

                    exit_gps_coords_temp.X = Math.Round(rc_xyz.X - targetposition.X, 2);
                    exit_gps_coords_temp.Y = Math.Round(rc_xyz.Y - targetposition.Y, 2);
                    exit_gps_coords_temp.Z = Math.Round(rc_xyz.Z - targetposition.Z, 2);

                    remoteControlActual.ClearWaypoints();
                    remoteControlActual.AddWaypoint(exit_gps_coords_temp, "exit shaft");
                    droneStatus = 18;
                    droneStatusOutput = "Exiting mineshaft";
                }
            }
            if (miningStage == 8 && exitWaypointAdjusted && !exitSequenceComplete && wasMining && miningInitialised && requestExit && !isAutopiloting && isUndocked)
            {
                miningStage = 9;
                remoteControlActual.SetCollisionAvoidance(false);
                remoteControlActual.SetDockingMode(false);
                remoteControlActual.SetAutoPilotEnabled(!navinst);
                droneStatus = 18;
                droneStatusOutput = "Exiting mineshaft";
            }
            if (miningStage == 9 && exitWaypointSet && !exitSequenceComplete && wasMining && miningInitialised && requestExit && rc_xyz != tgt_drill_exit && remoteControlActual.CurrentWaypoint.Name != "exit shaft" && !isAutopiloting && isUndocked)
            {
                remoteControlActual.SetCollisionAvoidance(false);
                remoteControlActual.SetDockingMode(false);
                remoteControlActual.SetAutoPilotEnabled(!navinst);
                droneStatus = 18;
                droneStatusOutput = "Exiting mineshaft";
                if (dockingStage > 0)
                {
                    droneStatusOutput = "Returning to dock";
                }
            }
            if (miningStage == 9 && exitWaypointSet && !exitSequenceComplete && wasMining && miningInitialised && requestExit && rc_xyz != tgt_drill_exit && !remoteControlActual.IsAutoPilotEnabled && !isAutopiloting && isUndocked)
            {
                remoteControlActual.SetCollisionAvoidance(false);
                remoteControlActual.SetDockingMode(false);
                remoteControlActual.SetAutoPilotEnabled(!navinst);
                droneStatus = 18;
                droneStatusOutput = "Exiting mineshaft reloading WP";
                if (dockingStage > 0)
                {
                    droneStatusOutput = "Returning to dock";
                }
            }
            if (miningStage == 9 && exitWaypointSet && !exitSequenceComplete && wasMining && miningInitialised && !remoteControlActual.IsAutoPilotEnabled && !isAutopiloting && isUndocked && exitWaypointAdjusted)
            {
                miningStage = 7;
                exitWaypointAdjusted = false;
                droneStatusOutput = "Exiting mineshaft reloading WP 2";
                if (dockingStage > 0)
                {
                    droneStatusOutput = "Returning to dock";
                }
            }
            double rc_cew_x = tgt_drill_exit.X;
            double rc_cew_y = tgt_drill_exit.Y;
            double rc_cew_z = tgt_drill_exit.Z;

            if (miningStage == 9 && !exitSequenceComplete && exitWaypointSet && rc_xyz.X >= tgt_drill_exit.X - nav_prec2 && rc_xyz.X <= tgt_drill_exit.X + nav_prec2 && rc_xyz.Y >= tgt_drill_exit.Y - nav_prec2 && rc_xyz.Y <= tgt_drill_exit.Y + nav_prec2 && rc_xyz.Z >= tgt_drill_exit.Z - nav_prec2 && rc_xyz.Z <= tgt_drill_exit.Z + nav_prec2 && miningInitialised && wasMining && requestExit && !isAutopiloting && isUndocked)
            {
                miningStage = 10;
                exitSequenceComplete = true;
                exitWaypointSet = true;
                remoteControlActual.SetCollisionAvoidance(false);
                remoteControlActual.SetDockingMode(false);
                remoteControlActual.SetAutoPilotEnabled(false);
                droneStatus = 19;
                droneStatusOutput = "Exit Clear";
            }
            if (miningStage == 9 && !exitSequenceComplete && exitWaypointSet && distance_current >= (drillSetLength + drill_el) && miningInitialised && wasMining && requestExit && !isAutopiloting && isUndocked)
            {
                miningStage = 10;
                exitSequenceComplete = true;
                exitWaypointSet = true;
                remoteControlActual.SetCollisionAvoidance(false);
                remoteControlActual.SetDockingMode(false);
                remoteControlActual.SetAutoPilotEnabled(false);
                droneStatus = 19;
                droneStatusOutput = "Exit Clear";
            }
            if (miningStage == 9 && !exitSequenceComplete && exitWaypointSet && rc_xyz.X >= exit_gps_coords_temp.X - nav_prec && rc_xyz.X <= exit_gps_coords_temp.X + nav_prec && rc_xyz.Y >= exit_gps_coords_temp.Y - nav_prec && rc_xyz.Y <= exit_gps_coords_temp.Y + nav_prec && rc_xyz.Z >= exit_gps_coords_temp.Z - nav_prec && rc_xyz.Z <= exit_gps_coords_temp.Z + nav_prec && miningInitialised && wasMining && requestExit && !isAutopiloting && isUndocked)
            {
                miningStage = 7;
                exitWaypointAdjusted = false;
                remoteControlActual.SetCollisionAvoidance(false);
                remoteControlActual.SetDockingMode(false);
                remoteControlActual.SetAutoPilotEnabled(false);
                droneStatus = 19;
                droneStatusOutput = "Getting next WP";
            }
            if (miningStage == 9 && wasMining && remoteControlActual.CurrentWaypoint.Name != "exit shaft" && requestExit && miningInitialised && !exitWaypointSet && !exitSequenceComplete && stopState && !isAutopiloting && isUndocked)
            {
                remoteControlActual.ClearWaypoints();
                remoteControlActual.AddWaypoint(exit_gps_coords_temp, "exit shaft");
                remoteControlActual.SetCollisionAvoidance(false);
                remoteControlActual.SetDockingMode(false);
                remoteControlActual.SetAutoPilotEnabled(!navinst);
                droneStatusOutput = "Exiting mineshaft";

            }
            if (miningStage == 10 && exitWaypointSet && exitSequenceComplete && miningInitialised && requestExit && !isAutopiloting && isUndocked)
            {
                miningStage = 11;
                remoteControlActual.ClearWaypoints();
                exitWaypointSet = false;
                exitSequenceComplete = false;
                requestExit = false;
                droneStatus = 20;
                droneStatusOutput = "Exit Clear";
            }
            if (miningStage == 11 && targetDepthAchieved && wasMining && miningInitialised && !requestExit && !isAutopiloting && isUndocked)
            {
                remoteControlActual.ClearWaypoints();
                exitWaypointSet = false;
                exitSequenceComplete = false;
                requestExit = false;
                if (wasMining)
                {
                    reset_mining = true;
                }
                if (!isDocking || !isUndocking)
                {
                    reset_ai();
                }
                droneStatus = 21;
                mainNavSequence = 0;
                if (collisionAvoidLightActual != null)
                {
                    collisionAvoidLightActual.Enabled = true;
                }
                if (collisionSenseEnabled)
                {
                    if (sensorActual != null)
                    {
                        if (!sensorActual.Enabled)
                        {
                            if (!sensorActual.Enabled) { sensorActual.Enabled = true; }
                        }
                    }
                }
                if (undockLightActual != null)
                {
                    if (!undockLightActual.Enabled)
                    {
                        undockLightActual.Enabled = true;
                    }
                }
                if (dockLightActual != null)
                {
                    if (dockLightActual.Enabled)
                    {
                        dockLightActual.Enabled = false;
                    }
                }

                if (autoDock)
                {
                    dockingStage = 1;
                    droneStatusOutput = "RTB Request A";
                    dockingReady = false;
                    no_speed_dock_delay_count = 0; // Reset docking delay
                    dock_delay_time = 0;
                    wasMining = false;
                }
                else
                {
                    dockingStage = 0;
                    miningStage = 12;
                    droneStatusOutput = "Preparing A";
                }

            }
            if (miningStage == 11 && !targetDepthAchieved && wasMining && miningInitialised && !requestExit && !isAutopiloting && isUndocked)
            {
                remoteControlActual.ClearWaypoints();
                exitWaypointSet = false;
                exitSequenceComplete = false;
                droneStatus = 22;
                requestExit = false;
                mainNavSequence = 0;
                if (wasMining)
                {
                    reset_mining = true;
                }
                if (!isDocking || !isUndocking)
                {
                    reset_ai();
                }
                if (collisionAvoidLightActual != null)
                {
                    if (!collisionAvoidLightActual.Enabled)
                    {
                        collisionAvoidLightActual.Enabled = true;
                    }
                }
                if (precModeLightActual != null)
                {
                    if (precModeLightActual.Enabled)
                    {
                        precModeLightActual.Enabled = false;
                    }
                }
                if (collisionSenseEnabled)
                {
                    if (sensorActual != null)
                    {
                        if (!sensorActual.Enabled)
                        {
                            if (!sensorActual.Enabled) { sensorActual.Enabled = true; }
                        }
                    }
                }
                if (undockLightActual != null)
                {
                    if (!undockLightActual.Enabled)
                    {
                        undockLightActual.Enabled = true;
                    }
                }
                if (dockLightActual != null)
                {
                    if (dockLightActual.Enabled)
                    {
                        dockLightActual.Enabled = false;
                    }
                }

                if (autoDock)
                {
                    dockingStage = 1;
                    droneStatusOutput = "RTB Request B";
                    dockingReady = false;
                    no_speed_dock_delay_count = 0; // Reset docking delay
                    dock_delay_time = 0;
                }
                else
                {
                    dockingStage = 0;
                    miningStage = 13;
                    droneStatusOutput = "Preparing B";
                }

            }
            if (miningStage == 12 && wasMining && miningInitialised && !requestExit && !isAutopiloting && isUndocked && targetDepthAchieved)
            {
                no_speed_dock_delay_count = 0; // Reset docking delay
                dock_delay_time = 0;
                droneStatusOutput = "RTB Ready A";
                droneStatus = 26;
                dockingReady = true;
            }
            if (miningStage == 13 && wasMining && miningInitialised && !requestExit && !isAutopiloting && isUndocked && !targetDepthAchieved)
            {
                no_speed_dock_delay_count = 0; // Reset docking delay
                dock_delay_time = 0;
                droneStatusOutput = "RTB Ready B";
                droneStatus = 27;
                dockingReady = true;
            }
            #endregion
        }

        private void Calculate_miningCoords()
        {
            // Initial position assignment
            mining_gps_coords.X = mining_gps_coords_temp.X;
            mining_gps_coords.Y = mining_gps_coords_temp.Y;
            mining_gps_coords.Z = mining_gps_coords_temp.Z;

            // Calculate direction vector
            Vector3D drillDirection = tgt_drill_end - tgt_drill_start;
            double pathLength = drillDirection.Length();

            // Normalize direction vector with safety check
            if (pathLength == 0)
            {
                sbtext.AppendLine("Error: Drill path length is zero!");
                return;
            }
            drillDirection.Normalize();

            // Calculate current position relative to start
            Vector3D currentOffset = rc_xyz - tgt_drill_start;

            // Project current position onto drill path
            double progressAlongPath = Vector3D.Dot(currentOffset, drillDirection);
            double fraction = progressAlongPath / pathLength;

            // Clamp fraction to valid range [0,1] if needed
            fraction = Math.Max(0, Math.Min(1, fraction));

            // Calculate expected position
            Vector3D expectedPos = tgt_drill_start + drillDirection * progressAlongPath;

            // Calculate perpendicular drift
            Vector3D driftVector = rc_xyz - expectedPos;
            double driftAlongPath = Vector3D.Dot(driftVector, drillDirection);

            // Get pure perpendicular drift
            Vector3D perpendicularDrift = driftVector - (drillDirection * driftAlongPath);
            double xyDrift = perpendicularDrift.Length(); // Using full 3D drift

            // Correction logic
            if (xyDrift > termnationPrecision * 2)
            {
                sbtext.AppendLine($"Drift: {xyDrift:F2}m - Correcting");
                mining_gps_coords = expectedPos;
            }
        }

        private void InitializeMining_Coordinates()
        {

            if (targetAlignmentValid)
            {
                direction = Vector3D.Normalize(new Vector3D(-(main_gps_coords - crnt_tgt_align)));
            }
            if (!targetAlignmentValid)
            {
                direction = Vector3D.Normalize(new Vector3D(gravity));
            }
            Vector3D targetposition = direction * req_dist;
            mining_gps_coords_temp.X = Math.Round(rc_xyz.X + targetposition.X, 2);
            mining_gps_coords_temp.Y = Math.Round(rc_xyz.Y + targetposition.Y, 2);
            mining_gps_coords_temp.Z = Math.Round(rc_xyz.Z + targetposition.Z, 2);

        }

        public void docking_management(bool canDock, bool autoDock)
        {

            if (!canDock)
            {
                if (dockingStage > 0)
                {
                    dockingStage = 0;
                }
                //early return if docking is disabled
                return;
            }
            if (((canDock) && dockingStage == 0 && !isDocked) || ((canDock) && dockingStage == 3 && !isDocked && (((!ai_task_dock_actual.GetValue<bool>(p1) && !ai_task_dock_actual.GetValue<bool>(abr)) && droneStatusOutput == "Idle"))))
            {
                dockingStage = 1;
                if (!switchedThrustersOn)
                {
                    Thruster_Management(true);
                    switchedThrustersOn = true;
                    switchedThrustersOff = false;
                }
                if (ai_move_actual != null)
                {
                    if (ai_move_actual.SpeedLimit != nav_speed)
                    {
                        ai_move_actual.SpeedLimit = nav_speed;
                    }
                }
            }
            #region docking_management
            if (resetLightActual != null)
            {
                if (resetLightActual.Enabled && dockingStage > 0)
                {
                    if (connectorActual != null)
                    {
                        connectorActual.Enabled = false;
                    }
                    reset_ai();
                    if (resetLightActual.Enabled)
                    {
                        if (connectorActual != null)
                        {
                            connectorActual.Enabled = true;
                        }
                        resetLightActual.Enabled = false;
                    }
                    dockingStage = 1;
                    droneStatusOutput = "Reset Docking Sequence";
                    if (!undockLightActual.Enabled)
                    {
                        undockLightActual.Enabled = true;
                    }
                    if (dockLightActual.Enabled)
                    {
                        dockLightActual.Enabled = false;
                    }
                }
            }
            if (precModeLightActual != null)
            {
                if (dockingStage > 0 && precModeLightActual.Enabled)
                {
                    if (collisionSenseEnabled)
                    {
                        if (sensorActual != null)
                        {
                            if (sensorActual.Enabled)
                            {
                                if (sensorActual.Enabled) { sensorActual.Enabled = false; }
                            }
                        }
                    }
                    if (collisionAvoidLightActual != null)
                    {
                        if (collisionAvoidLightActual.Enabled)
                        {
                            collisionAvoidLightActual.Enabled = false;
                        }
                    }
                    if (ai_move_actual != null)
                    {
                        if (!ai_move_actual.PrecisionMode)
                        {
                            ai_move_actual.PrecisionMode = true;
                        }
                        if (ai_move_actual.CollisionAvoidance)
                        {
                            ai_move_actual.CollisionAvoidance = false;
                        }
                    }
                }
            }
            if (precModeLightActual != null)
            {
                if (dockingStage > 0 && !precModeLightActual.Enabled)
                {
                    if (ai_move_actual != null)
                    {
                        ai_move_actual.PrecisionMode = false;
                    }
                }
            }
            if (dockingStage > 0 && collisionAvoidLightActual.Enabled)
            {
                if (ai_move_actual != null)
                {
                    if (!ai_move_actual.CollisionAvoidance)
                    {
                        ai_move_actual.CollisionAvoidance = true;
                    }
                }
            }

            if (dockingStage == 1)
            {
                if (connectorActual != null)
                {
                    if (!connectorActual.Enabled)
                    {
                        connectorActual.Enabled = true;
                    }
                }
                StDrlOnOff(false, cnvyrsON);
                if (initiateDrills)
                {
                    initiateDrills = false;
                }
                if (undockLightActual != null)
                {
                    if (!undockLightActual.Enabled)
                    {
                        if (connectorActual != null)
                        {
                            if (!connectorActual.IsConnected && connectorActual.Status != MyShipConnectorStatus.Connectable)
                            {
                                undockLightActual.Enabled = true;
                            }
                        }
                    }
                }
                if (connectorActual != null)
                {
                    if (connectorActual.Status != MyShipConnectorStatus.Connectable && dockingStage == 1)
                    {

                        if (!skip_prec_mode)
                        {
                            if (ai_move_actual != null)
                            {
                                if (!ai_move_actual.PrecisionMode)
                                {
                                    ai_move_actual.PrecisionMode = true;
                                }
                            }
                        }
                        if (ai_move_actual != null)
                        {
                            if (!ai_move_actual.CollisionAvoidance)
                            {
                                ai_move_actual.CollisionAvoidance = true;
                            }
                            if (ai_move_actual.SpeedLimit != nav_speed)
                            {
                                ai_move_actual.SpeedLimit = nav_speed;
                            }
                        }
                        if (ai_move_actual != null)
                        {
                            if ((skip_prec_mode && ai_move_actual.CollisionAvoidance) || (!skip_prec_mode && ai_move_actual.PrecisionMode && ai_move_actual.CollisionAvoidance))
                            {
                                if (!ai_move_actual.GetValue<bool>(abr))
                                {
                                    ai_move_actual.GetActionWithName(ab1).Apply(ai_move_actual);
                                }
                                if (ai_task_dock_actual != null)
                                {
                                    if (!ai_task_dock_actual.GetValue<bool>(abr))
                                    {
                                        ai_task_dock_actual.GetActionWithName(ab1).Apply(ai_task_dock_actual);
                                    }
                                    if (!ai_task_dock_actual.GetValue<bool>(p1))
                                    {
                                        ai_task_dock_actual.GetActionWithName(pon).Apply(ai_task_dock_actual);
                                    }
                                }
                                if (collisionAvoidLightActual != null)
                                {
                                    if (!collisionAvoidLightActual.Enabled)
                                    {
                                        collisionAvoidLightActual.Enabled = true;
                                    }
                                }
                                if (collisionSenseEnabled)
                                {
                                    if (sensorActual != null)
                                    {
                                        if (!sensorActual.Enabled)
                                        {
                                            if (!sensorActual.Enabled) { sensorActual.Enabled = true; }
                                        }
                                    }
                                }
                            }
                        }
                        dockingStage = 2;
                        droneStatusOutput = "Docking";
                    }
                    else
                    {
                        dockingStage = 2;
                        droneStatusOutput = "Returning to dock";
                    }
                }
            }
            if (AInav_fault && dockingStage == 2)
            {
                if (precModeLightActual != null)
                {
                    if (precModeLightActual.Enabled)
                    {
                        precModeLightActual.Enabled = false;
                    }
                }
                if (collisionAvoidLightActual != null)
                {
                    if (!collisionAvoidLightActual.Enabled)
                    {
                        collisionAvoidLightActual.Enabled = true;
                    }
                }
            }
            if (dockingStage == 2 && AInav_fault)
            {
                if (resetLightActual != null)
                {
                    if (resetLightActual.Enabled)
                    {
                        if (AInav_fault)
                        {
                            AInav_fault = false;
                        }
                    }

                }
                else
                {
                    AInav_fault = false;
                }
            }
            if (dockingStage == 2)
            {
                IMyAutopilotWaypoint myWaypoint = ai_move_actual.CurrentWaypoint;

                if (resetLightActual != null && connectorActual != null && sensorActual != null && precModeLightActual != null)
                {
                    if ((connectorActual.Status != MyShipConnectorStatus.Connectable
                        && !precModeLightActual.Enabled
                        && sensorActual.Enabled
                        && !resetLightActual.Enabled
                        && currentSpeed < currentSpeedNotMovingThreshold
                        && no_speed_dock_delay_count < no_speed_dock_delay_limit)
                            || (connectorActual.Status != MyShipConnectorStatus.Connectable
                            && precModeLightActual.Enabled
                            && !sensorActual.Enabled
                            && !resetLightActual.Enabled
                            && currentSpeed < currentSpeedNotMovingThreshold
                            && no_speed_dock_delay_count < no_speed_dock_delay_limit)
                        || (connectorActual.Status != MyShipConnectorStatus.Connectable
                        && precModeLightActual.Enabled
                        && !sensorActual.Enabled
                        && !resetLightActual.Enabled
                        && currentSpeed < currentSpeedNotMovingThreshold
                        && no_speed_dock_delay_count < no_speed_dock_delay_limit)
                            || (connectorActual.Status != MyShipConnectorStatus.Connectable
                            && !precModeLightActual.Enabled
                            && sensorActual.Enabled
                            && !resetLightActual.Enabled
                            && currentSpeed < currentSpeedNotMovingThreshold
                            && no_speed_dock_delay_count < no_speed_dock_delay_limit)
                                            )
                    {
                        no_speed_dock_delay_count++;
                        dock_delay_time = Math.Round(((double)no_speed_dock_delay_count * (double)10 * game_tick_length) / (double)1000, 1);
                    }
                }
                if (ai_move_actual != null)
                {
                    if (ai_move_actual.SpeedLimit != nav_speed)
                    {
                        ai_move_actual.SpeedLimit = nav_speed;
                    }
                    if (!ai_move_actual.GetValue<bool>(abr))
                    {
                        ai_move_actual.GetActionWithName(ab1).Apply(ai_move_actual);
                    }
                }
                StDrlOnOff(false, cnvyrsON);
                if (initiateDrills)
                {
                    initiateDrills = false;
                }
                if (connectorActual != null)
                {
                    if (connectorActual.Status == MyShipConnectorStatus.Connectable && dockingStage == 2)
                    {
                        connectorActual.Connect();
                        reset_mining = true;
                        droneStatusOutput = "Docked";
                        undocking_stage = 0;
                    }
                    if (connectorActual.Status == MyShipConnectorStatus.Connected && dockingStage == 2)
                    {
                        Thruster_Management(false);
                        dockingStage = 3;
                    }
                }
                if (connectorActual != null && ai_task_dock_actual != null && precModeLightActual != null)
                {
                    if (connectorActual.Status != MyShipConnectorStatus.Connectable && dockingStage == 2 && !no_speed_ready_dock && (!ai_task_dock_actual.GetValue<bool>(p1) && (!ai_task_dock_actual.GetValue<bool>(abr))) && !precModeLightActual.Enabled) //checking if not docking properly when not in precision mode to restart
                    {
                        if (collisionSenseEnabled)
                        {
                            if (sensorActual != null)
                            {
                                if (!sensorActual.Enabled)
                                {
                                    sensorActual.Enabled = !sensorActual.Enabled;
                                }
                            }
                        }
                        if (ai_move_actual != null)
                        {
                            if (!ai_move_actual.GetValue<bool>(abr))
                            {
                                ai_move_actual.GetActionWithName(ab1).Apply(ai_move_actual);
                            }
                        }
                        if (!ai_task_dock_actual.GetValue<bool>(abr))
                        {
                            ai_task_dock_actual.GetActionWithName(ab1).Apply(ai_task_dock_actual);
                        }

                        if (!ai_task_dock_actual.GetValue<bool>(p1))
                        {
                            //ai_task_dock_actual.ApplyAction(p1);
                            ai_task_dock_actual.GetActionWithName(pon).Apply(ai_task_dock_actual);
                        }

                        droneStatusOutput = "Docking";
                    }

                }

                if (connectorActual != null && ai_task_dock_actual != null)
                {
                    if (connectorActual.Status != MyShipConnectorStatus.Connectable && dockingStage == 2 && no_speed_ready_dock && (ai_task_dock_actual.GetValue<bool>(p1) && (ai_task_dock_actual.GetValue<bool>(abr))))
                    {
                        if (precModeLightActual != null)
                        {
                            precModeLightActual.Enabled = false;
                        }

                        if (precMflip)
                        {
                            if (ai_move_actual != null)
                            {
                                if (ai_move_actual.PrecisionMode && !precMflipped)
                                {
                                    ai_move_actual.PrecisionMode = false;
                                    precMflipped = true;
                                }
                                if (!ai_move_actual.PrecisionMode && !precMflipped)
                                {
                                    ai_move_actual.PrecisionMode = true;
                                    precMflipped = true;
                                }
                            }

                        }
                        if (!ai_task_dock_actual.GetValue<bool>(abr))
                        {
                            ai_task_dock_actual.GetActionWithName(ab1).Apply(ai_task_dock_actual);
                        }

                        if (!ai_task_dock_actual.GetValue<bool>(p1))
                        {
                            //ai_task_dock_actual.ApplyAction(p1);
                            ai_task_dock_actual.GetActionWithName(pon).Apply(ai_task_dock_actual);
                        }

                    }
                }


                // To do:check waypoint name from move block - if null or blank for time delay then reset docking sequence
                //get terminal properties
                if (connectorActual != null && ai_task_dock_actual != null)
                {
                    if (connectorActual.Status != MyShipConnectorStatus.Connectable && dockingStage == 2 && no_speed_ready_dock && (!ai_task_dock_actual.GetValue<bool>(p1) && (!ai_task_dock_actual.GetValue<bool>(abr) || (ai_task_dock_actual.GetValue<bool>(abr)))))
                    {
                        if (resetLightActual != null)
                        {
                            if (!resetLightActual.Enabled)
                            {
                                resetLightActual.Enabled = true;
                            }
                        }
                        if (precModeLightActual != null)
                        {
                            precModeLightActual.Enabled = false;
                        }

                        if (precMflip)
                        {
                            if (ai_move_actual != null)
                            {
                                if (ai_move_actual.PrecisionMode && !precMflipped)
                                {
                                    ai_move_actual.PrecisionMode = false;
                                    precMflipped = true;
                                }
                                if (!ai_move_actual.PrecisionMode && !precMflipped)
                                {
                                    ai_move_actual.PrecisionMode = true;
                                    precMflipped = true;
                                }
                            }

                        }

                    }
                }


            }

            if (precMflip && dockingStage == 2)
            {
                if (ai_move_actual != null)
                {
                    if (ai_move_actual.PrecisionMode && precMflipped)
                    {
                        ai_move_actual.PrecisionMode = false;
                        precMflipped = false;
                        precMflip = false;
                    }
                    if (!ai_move_actual.PrecisionMode && precMflipped)
                    {
                        ai_move_actual.PrecisionMode = true;
                        precMflipped = false;
                        precMflip = false;
                    }
                }
            }

                if (dockingStage == 3 && isDocked)
                {
                    no_speed_dock_delay_count = 0;
                    StDrlOnOff(false, cnvyrsON);
                    if (initiateDrills)
                    {
                        initiateDrills = false;
                    }
                    if (!thrustGroupPresent)
                    {
                        if (timerBlockTOFFActual != null)
                        {
                            if (!timerBlockTOFFActual.Enabled)
                            {
                                timerBlockTOFFActual.Enabled = true;
                            }
                            timerBlockTOFFActual.Trigger();
                        }
                    }
                    else
                    {
                        Thruster_Management(false);
                    }

                    reset_ai();
                    if (resetLightActual != null)
                    {
                        if (resetLightActual.Enabled)
                        {
                            resetLightActual.Enabled = false;
                        }
                    }
                    if (dockLightActual != null)
                    {
                        if (!dockLightActual.Enabled)
                        {
                            dockLightActual.Enabled = true;
                        }
                    }
                    if (undockLightActual != null)
                    {
                        if (undockLightActual.Enabled)
                        {
                            undockLightActual.Enabled = false;
                        }
                    }
                    if (precModeLightActual != null)
                    {
                        if (precModeLightActual.Enabled)
                        {
                            precModeLightActual.Enabled = false;
                        }
                    }
                    if (recharge_request_battery)
                    {
                        for (int i = 0; i < battery_tag.Count; i++)
                        {
                            if (!batteryRechargeModeSet)
                            {
                                batteryupdate(ChargeMode.Recharge);
                                batteryRechargeModeSet = true;
                                batteryAutochargeSet = false;
                            }
                            droneStatusOutput = "Recharging";
                        }
                    }
                    if (!recharge_request_battery)
                    {
                        if (!batteryAutochargeSet)
                        {
                            batteryupdate(ChargeMode.Auto);
                            batteryRechargeModeSet = false;
                            batteryAutochargeSet = true;
                        }
                    }
                    if (recharge_request_tank && !ignore_Htank)
                    {
                        if (!tankRechargeModeSet)
                        {
                            tankupdate(true);
                            tankAutochargeSet = false;
                            tankRechargeModeSet = true;
                        }
                        droneStatusOutput = "Recharging";

                    }
                    if (!recharge_request_tank && !ignore_Htank)
                    {
                        if (!tankAutochargeSet)
                        {
                            tankupdate(false);
                            tankAutochargeSet = true;
                            tankRechargeModeSet = false;
                        }
                    }
                    if (!recharge_request)
                    {
                        dockingStage = 0;
                    }
                }
                if (dockingStage >= 1 && dockingStage <= 2 && stopState && isDocking && !isDocked)
                {
                    reset_ai();
                    if (resetLightActual != null)
                    {
                        if (resetLightActual.Enabled)
                        {
                            resetLightActual.Enabled = false;
                        }
                    }
                    dockingStage = 0;
                }
                #endregion

            }
        
        public void connector_state_management(bool dockingReady)
        {
            if (connectorActual == null)
            {
                dockingReady = false;
                return;
            }
            if (connectorActual.IsConnected && dockingReady)
            {
                dockingReady = false;
            }
            #region connector_state_management
            if ((connectorActual.IsConnected && ignore_Htank) || (connectorActual.IsConnected && !ignore_Htank))
            {
                if (!tankRechargeModeSet)
                {
                    tankupdate(true);
                    tankAutochargeSet = false;
                    tankRechargeModeSet = true;
                }

            }
            if (!connectorActual.IsConnected && ignore_Htank)
            {
                if (!tankAutochargeSet)
                {
                    tankupdate(false);
                    tankAutochargeSet = true;
                    tankRechargeModeSet = false;
                }
            }
            if ((connectorActual.IsConnected && cargoFullAchieved) || (connectorActual.IsConnected && !cargoIsEmpty))
            {
                droneStatusOutput = "Docked Unloading";

                if (collisionSenseEnabled)
                {
                    if (sensorActual != null)
                    {
                        if (sensorActual.Enabled) { sensorActual.Enabled = false; }
                    }
                }
                if (collisionAvoidLightActual != null)
                {
                    if (collisionAvoidLightActual.Enabled)
                    {
                        collisionAvoidLightActual.Enabled = false;
                    }
                }
                if (resetLightActual != null)
                {
                    if (resetLightActual.Enabled)
                    {
                        resetLightActual.Enabled = false;
                    }
                }
            }
            if (connectorActual.IsConnected && recharge_request)
            {
                droneStatusOutput = "Docked Recharging";
                if (collisionSenseEnabled)
                {
                    if (sensorActual != null)
                    {
                        if (sensorActual.Enabled) { sensorActual.Enabled = false; }
                    }
                }
                if (collisionAvoidLightActual != null)
                {
                    if (collisionAvoidLightActual.Enabled)
                    {
                        collisionAvoidLightActual.Enabled = false;
                    }
                }
                if (resetLightActual != null)
                {
                    if (resetLightActual.Enabled)
                    {
                        resetLightActual.Enabled = false;
                    }
                }
            }
            if (connectorActual.IsConnected && !undockState && !cargoFullAchieved && cargoIsEmpty && !recharge_request)
            {
                droneStatusOutput = "Docked Idle";
                if (collisionSenseEnabled)
                {
                    if (sensorActual != null)
                    {
                        if (sensorActual.Enabled) { sensorActual.Enabled = false; }
                    }
                }
                if (collisionAvoidLightActual != null)
                {
                    if (collisionAvoidLightActual.Enabled)
                    {
                        collisionAvoidLightActual.Enabled = false;
                    }
                }
                if (resetLightActual != null)
                {
                    if (resetLightActual.Enabled)
                    {
                        resetLightActual.Enabled = false;
                    }
                }
            }
            if (!connectorActual.IsConnected)
            {
                if (dockLightActual != null)
                {
                    if (dockLightActual.Enabled)
                    {
                        dockLightActual.Enabled = false;
                    }
                }
            }
            if (connectorActual.IsConnected)
            {
                if (dockLightActual != null)
                {
                    if (!dockLightActual.Enabled)
                    {
                        dockLightActual.Enabled = true;
                    }
                }
                if (undockLightActual != null)
                {
                    if (undockLightActual.Enabled)
                    {
                        undockLightActual.Enabled = false;
                    }
                }
                if (!switchedThrustersOff && thrustGroupPresent)
                {
                    Thruster_Management(false);
                    switchedThrustersOff = true;
                    switchedThrustersOn = false;
                }
            }
            #endregion

        }

        public void nagivation_movement_check()
        {

            #region nagivation_movement_check
            if (navigation_reset_delay)
            {
                navigation_reset_delay = false;
                navigation_reset_delay_time = Math.Round(((double)no_speed_count_navigation_reset_delay_count * (double)10 * game_tick_length) / (double)1000, 1);
                no_speed_count_navigation_reset_delay_count = 0;
            }
            #endregion

        }

        public void undock_delay_check()
        {
            if ((!isUndocking || isUndocked) && undock_delay_time > 0.0)
            {
                undock_delay_time = 0;
                no_speed_undock_delay_count = 0;
            }
            #region undock_delay_check
            if (no_speed_ready_undock)
            {
                undock_delay_time = Math.Round(((double)no_speed_undock_delay_count * (double)10 * game_tick_length) / (double)1000, 1);
                no_speed_ready_undock = false;
                no_speed_undock_delay_count = 0;
            }
            #endregion

        }
        public void dock_delay_check()
        {
            if ((!isDocking || isDocked) && dock_delay_time > 0.0)
            {
                dock_delay_time = 0;
                no_speed_dock_delay_count = 0;
            }
            #region dock_delay_check
            if (no_speed_ready_dock)
            {
                dock_delay_time = Math.Round(((double)no_speed_dock_delay_count * (double)10 * game_tick_length) / (double)1000, 1);
                no_speed_ready_dock = false;
                no_speed_dock_delay_count = 0;
            }
            #endregion

        }

        public void drone_message_transmission_management(bool autoDock, IMyRemoteControl rc_actual, IMyRadioAntenna antenna_actual, bool dockingReady)
        {

            string dataTransmissionOut;
            if (antenna_actual == null) { sbtext.AppendLine("Error: antenna is null in drone_message_transmission_management"); return; }
            if (rc_actual == null) { sbtext.AppendLine("Error: remote control is null in drone_message_transmission_management"); return; }
            #region drone_transmission_response_management
            if (transmit_delay && pinged)
            {
                response_time = Math.Round(((double)t_count * (double)10 * game_tick_length) / (double)1000, 1);
                transmit_delay = false;
                t_count = 0;
            }

            if (pinged)
            {
                const string baseFormat = "{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}:{11}:{12}:{13}:{14}:{15}:{16}:{17}:{18}:{19}:{20}:{21}:{22}:{23}";
                sb.Clear().EnsureCapacity(128);
                sb.AppendFormat(baseFormat, D_I_N,
                    droneDamageStatus, tunnelSequenceFinished, droneStatusOutput,
                    isDocked, isUndocked, isAutopiloting,
                    rc_actual.IsAutoPilotEnabled,
                    Math.Round(rc_xyz.X, 2), Math.Round(rc_xyz.Y, 2), Math.Round(rc_xyz.Z, 2),
                    drillSetLength, Math.Round(distance_current, 2), Math.Round(drillSetLength - ignoreDistance, 2),
                    Math.Round(percent_battery_power, 2), Math.Round(pcnt_gas_tank, 2), Math.Round(total_percent_cargo_used, 2),
                    gpsIndex, cargoFullAchieved, recharge_request,
                    autoDock, dockingReady, commandRequest, ver
                    );
                dataTransmissionOut = sb.ToString();
                IGC.SendBroadcastMessage(tx_ch, dataTransmissionOut, TransmissionDistance.TransmissionDistanceMax); // Direct sb use
                sbtext.AppendLine("Transmission sent");
                dataTransmissionOut = "";
                pinged = false;
                pingedMessageDataIn = "";
            }
            #endregion

        }
        public void rc_navigation_init()
        {

            #region rc_navigation_init_nav_or_mine
            if (custom_data_read == 1 && cmd_read_ack == 0)
            {
                cmd_read_ack = 1;
                clr_cords = false;
            }

            if (!clr_cords && custom_data_read == 1)
            {
                clr_cords = true;
                add_nav_Waypoint_mn = false;
                add_mine_waypoint = false;
                remoteControlActual.ClearWaypoints();
                remoteControlActual.SetAutoPilotEnabled(false);
            }


            if (clr_cords && custom_data_read == 1)
            {
                if (navState && commandChanged && isUndocked)
                {
                    mainNavSequence = 1;
                    droneStatusOutput = "Nav";
                }
                if (mineState && commandChanged && isUndocked && !dockingReady)
                {
                    miningStage = 0;
                    miningInitialised = false;
                }
            }
            #endregion

        }
        public void Drone_Local_Status_Reporting()
        {

            #region drone_status_local_report
            // Pre-calculate commonly used values to avoid repeated Math.Round calls
            double runtimePercent = Math.Round((_Runtime / game_tick_length) * 100.0, 3);
            double runtimeMs = Math.Round(_Runtime, 3);
            double cargoPercent = Math.Round(total_percent_cargo_used, 2);
            double batteryPercent = Math.Round(percent_battery_power, 2);
            double tankPercent = hydrogen_tank_tag.Count > 0 ? Math.Round(pcnt_gas_tank, 2) : 0;
            double mineDistance = Math.Round(distance_current, 2);
            double speed = Math.Round(spd, 2);
            double groundSpeed = Math.Round(currentSpeed, 2);            

            // Core status report
            sbtext.Append("Load: ").Append(runtimePercent).Append("% (").Append(runtimeMs).Append("ms) I#: ").Append(_Instruction).Append(" ").Append(nav_speed).Append('\n');
            sbtext.Append("Drone ID: ").Append(D_I_N.Replace("[", "[[").Replace("]", "]]")).Append(" # ").Append(droneDamageStatus).Append('\n');
            sbtext.Append("Status Ints: ").Append(drnst).Append('\n');
            sbtext.Append("Drone Status: ").Append(droneStatusOutput).Append('\n');
            sbtext.Append("Distance ID: ").Append(miningInitialised).Append('\n');
            sbtext.Append("Command seq: ").Append(commandRequest).Append(" AIFault: ").Append(AInav_fault).Append('\n');
            sbtext.Append("Cargo: ").Append(cargoPercent).Append("%  Full: ").Append(cargoFullAchieved).Append('\n');
            sbtext.Append("Charge: ").Append(batteryPercent).Append("%  Recharge: ").Append(recharge_request_battery).Append('\n');

            if (hydrogen_tank_tag.Count > 0)
            {
                sbtext.Append("HTank: ").Append(tankPercent).Append("%  Recharge: ").Append(recharge_request_tank).Append('\n');
            }

            sbtext.Append("Mine distance: ").Append(mineDistance).Append("m  Mine Start: ").Append(drillSetLength - ignoreDistance).Append("m\n");
            sbtext.Append("Mine: ").Append(mineState).Append(" - Stage: ").Append(miningStage).Append(" WM:").Append(wasMining).Append(" - Ed: ").Append(drill_el).Append('\n');
            sbtext.Append("Nav: ").Append(navState).Append(" - Stage: ").Append(mainNavSequence).Append('\n');
            sbtext.Append("Dock: ").Append(isDocked).Append(" - Stage: ").Append(dockingStage).Append(" DR: ").Append(dockingReady).Append('\n');
            sbtext.Append("Undock: ").Append(isUndocked).Append(" - Stage: ").Append(undocking_stage).Append('\n');
            sbtext.Append("Connected: ").Append(connectorActual.IsConnected).Append('\n');
            sbtext.Append("Depth Achieved: ").Append(targetDepthAchieved).Append('\n');
            sbtext.Append("Stopped: ").Append(stopState).Append('\n');
            sbtext.Append("Last response: ").Append(response_time).Append("s waiting: ").Append(transmit_delay).Append('\n');
            sbtext.Append("Undock timer: ").Append(undock_delay_time).Append("s ").Append(no_speed_ready_undock).Append('\n');
            sbtext.Append("Dock timer: ").Append(dock_delay_time).Append("s ").Append(no_speed_ready_dock).Append(" GRef: ").Append(gridrefreshcount).Append('\n');
            sbtext.Append("Nav timer: ").Append(navigation_reset_delay_time).Append("s ").Append(navigation_reset_delay).Append('\n');
            sbtext.Append("Speed: ").Append(speed).Append(" ").Append(groundSpeed).Append('\n');

            if (displays_tag.Count > 0)
            {
                for (int i = 0; i < displays_tag.Count; i++)
                {

                    if (displays_tag[i] != null)
                    {
                        visualiser = ((IMyTextSurfaceProvider)displays_tag[0]).GetSurface(0);
                        if (visualiser.ContentType != ContentType.TEXT_AND_IMAGE)
                        {
                            //Echo("Correcting visualiser display");
                            visualiser.ContentType = ContentType.TEXT_AND_IMAGE;
                            visualiser.FontSize = 0.66f;
                            visualiser.Font = "White";
                        }
                        visualiser.WriteText(sbtext.ToString());
                    }

                }
            }
            if (displays_debug.Count > 0)
            {
                for (int i = 0; i < displays_debug.Count; i++)
                {
                    if (displays_debug[i] != null)
                    {
                        visualiser = ((IMyTextSurfaceProvider)displays_debug[0]).GetSurface(0);
                        if (visualiser.ContentType != ContentType.TEXT_AND_IMAGE)
                        {
                            //Echo("Correcting visualiser display");
                            visualiser.ContentType = ContentType.TEXT_AND_IMAGE;
                            visualiser.FontSize = 0.66f;
                            visualiser.Font = "White";
                        }
                        visualiser.WriteText(sbtext.ToString());
                    }

                }
            }
            if (displays_tag.Count == 0)
            {
                Echo(sbtext.ToString());
            }
            sbtext.Clear();
            #endregion

        }

        public void function_delay_management()
        {

            #region function_delay_management
            t_count++;

            if (t_count >= transmit_time_limit)
            {
                transmit_delay = true;
            }
            if (no_speed_count_navigation_reset_delay_count >= no_speed_navigation_delay_limit)
            {
                navigation_reset_delay = true;
            }
            if (no_speed_undock_delay_count >= no_speed_undock_delay_limit)
            {
                no_speed_ready_undock = true;
            }
            if (no_speed_dock_delay_count >= no_speed_dock_delay_limit)
            {
                no_speed_ready_dock = true;
                if (!AInav_fault)
                {
                    AInav_fault = true;
                }
                //_updatedJob = true;
                /*
                if (!precMflip)
                {
                    precMflip = true;
                }
                */
            }
            #endregion

        }

        public void Thruster_Management(bool EnableOnOff)
        {
            IMyGridTerminalSystem gts = GridTerminalSystem as IMyGridTerminalSystem;
            thrusterGroup = gts.GetBlockGroupWithName(thrustGroupTag) as IMyBlockGroup;
            thrust_tag.Clear();
            gts.GetBlocksOfType<IMyThrust>(thrust_tag, b => b.CubeGrid == Me.CubeGrid);
            if (thrust_tag.Count > 0)
            {
                thrustGroupPresent = true;
            }
            else
            {
                setupIsComplete = false;
                thrustGroupPresent = false;
                return;
            }


            if (thrust_tag.Count > 0)
            {
                for (int i = 0; i < thrust_tag.Count; i++)
                {
                    if (thrust_tag[i] != null)
                    {
                        if (thrust_tag[i].Enabled != EnableOnOff)
                        {
                            thrust_tag[i].Enabled = EnableOnOff;
                        }
                    }
                }
            }
            else
            {
                if (thrusterGroup != null)
                {
                    sbtext.AppendLine($"Thrusters not found in {thrusterGroup.Name.Replace("[", "[[").Replace("]", "]]")}. Please add thrusters");
                }
                return;

            }
        }

        void batteryupdate(ChargeMode chargetype)
        {
            if (battery_tag.Count > 0)
            {
                for (int i = 0; i < battery_tag.Count; i++)
                {
                    if (battery_tag[i] != null)
                    {
                        if (battery_tag[i].ChargeMode != chargetype)
                        {
                            battery_tag[i].ChargeMode = chargetype;
                        }
                    }
                }
            }
        }

        void tankupdate(bool stockpilemode)
        {
            if (hydrogen_tank_tag.Count > 0)
            {
                for (int i = 0; i < hydrogen_tank_tag.Count; i++)
                {
                    if (hydrogen_tank_tag[i] != null)
                    {
                        if (hydrogen_tank_tag[i].Stockpile != stockpilemode)
                        {
                            hydrogen_tank_tag[i].Stockpile = stockpilemode;
                        }
                    }
                }
            }
        }

        public void StoreRawInput(string inputString, IMyTerminalBlock block, string INI_SECTION = "GMDCJobData", string INI_KEY = "Jobinfo")
        {
            var iniBuilder = new MyIni();
            iniBuilder.Clear();
            if (iniBuilder.TryParse(block.CustomData.ToString()))
            {
                // 1. Correct MyIni.Set() usage: (Section, Key, Value)
                iniBuilder.Set(INI_SECTION, INI_KEY, inputString);
            }
            else
            {
                iniBuilder.Set(INI_SECTION, INI_KEY, inputString);
            }
            /*iniBuilder.Set(INI_SECTION, "droneTagname", droneTag);
            iniBuilder.Set(INI_SECTION, "droneidnum", drone_id_num);            
            iniBuilder.Set(INI_SECTION, "collisionSenseEnabled", collisionSenseEnabled);
            iniBuilder.Set(INI_SECTION, "cargoSenseEnabled", cargoSenseEnabled);
            iniBuilder.Set(INI_SECTION, "cargoSenseLimit", cargoSenseLimit);
            iniBuilder.Set(INI_SECTION, "drillterrainenabled", terrainclearEnable);
            iniBuilder.Set(INI_SECTION, "drillterrainoffset", terrainclearoffset);
            iniBuilder.Set(INI_SECTION, "keepterrain", terrainKeepMode);
            iniBuilder.Set(INI_SECTION, "damageReportingEnabled", damageReportingEnabled);
            iniBuilder.Set(INI_SECTION, "sensorleftlim", s_llm);
            iniBuilder.Set(INI_SECTION, "sensorrightlim", s_rlm);
            iniBuilder.Set(INI_SECTION, "sensorbottomlim", s_btlm);
            iniBuilder.Set(INI_SECTION, "sensortoplim", s_tlm);
            iniBuilder.Set(INI_SECTION, "sensorbacklim", s_bklm);
            iniBuilder.Set(INI_SECTION, "sensorfrontlim", s_flm);
            iniBuilder.Set(INI_SECTION, "ignorehydrogentank", ignore_Htank);
            iniBuilder.Set(INI_SECTION, "autoChargeMode", autoChargeMode);
            iniBuilder.Set(INI_SECTION, "gas_CHGhi", gas_CHGhi);
            iniBuilder.Set(INI_SECTION, "gas_CHGlow", gas_CHGlow);
            iniBuilder.Set(INI_SECTION, "bat_CHGhi", bat_CHGhi);
            iniBuilder.Set(INI_SECTION, "bat_CHGlow", bat_CHGlow);
            iniBuilder.Set(INI_SECTION, "drill_speed", drill_speed);
            iniBuilder.Set(INI_SECTION, "nav_speed", nav_speed);
            iniBuilder.Set(INI_SECTION, "exit_speed", exit_speed); */


            // Save to the Programmable Block's CustomData
            block.CustomData = iniBuilder.ToString();
            iniBuilder.Clear();
            //sbtext.AppendLine($"Raw input stored successfully in [{INI_SECTION}] {INI_KEY}.");
        }

        public void ClearAllNonEmptyLists()
        {
            ClearNonEmptyLists<IMyRemoteControl>(rc_all, rctag);
            ClearNonEmptyLists<IMySensorBlock>(sensor_all, sensor_tag);
            ClearNonEmptyLists<IMyCameraBlock>(cam_all, camera_tag);
            ClearNonEmptyLists<IMyShipConnector>(connector_all, connector_tag);
            ClearNonEmptyLists<IMyCargoContainer>(cargo_all, cargo_tag, cargo_sense);
            ClearNonEmptyLists<IMyRadioAntenna>(antenna_all, antenna_tag);
            ClearNonEmptyLists<IMyPathRecorderBlock>(flight_path_all, flight_path_dock_tag, flight_path_undock_tag);
            ClearNonEmptyLists<IMyFlightMovementBlock>(flight_move_all, flight_move_tag);
            ClearNonEmptyLists<IMyTimerBlock>(timer_block_all, timer_block_tON_tag, timer_block_tOFF_tag,
                                             timer_block_precM_tag, timer_block_undock_tag);
            ClearNonEmptyLists<IMyLightingBlock>(light_all, lightUndockTag, light_dock_tag,
                                                light_collision_avoid_tag, lightPrecMTag, lightResetTag, light_dmg_tag);
            ClearNonEmptyLists<IMyBatteryBlock>(battery_all, battery_tag);
            ClearNonEmptyLists<IMyGasTank>(hydrogen_tank_all, hydrogen_tank_tag);
            ClearNonEmptyLists<IMyShipDrill>(drill_all, drill_tag);
            ClearNonEmptyLists<IMyThrust>(thrust_all, thrust_tag);
            ClearNonEmptyLists<IMyGyro>(gyro_all, gyroTag);
            // Handle waypoints separately since it's a struct
            if (waypoints != null && waypoints.Count > 0)
            {
                waypoints.Clear();
            }
        }

        private void ClearNonEmptyLists<T>(params List<T>[] lists) where T : class
        {
            foreach (var list in lists)
            {
                if (list != null && list.Count > 0)
                {
                    list.Clear();
                }
            }
        }

        double ParseDouble(string[] data, int index, double defaultValue)
        {
            double result;
            if (index < data.Length && double.TryParse(data[index], out result)) return result;
            return defaultValue;
        }

        int ParseInt(string[] data, int index, int defaultValue)
        {
            int result;
            if (index < data.Length && int.TryParse(data[index], out result)) return result;
            return defaultValue;
        }

        double ParseAndValidate(string[] data, int index, out bool present)
        {
            double result;
            if (index < data.Length && !string.IsNullOrEmpty(data[index]) && double.TryParse(data[index], out result))
            {
                present = true;
                return result;
            }
            present = false;
            return 0.0;
        }
        //end program

       
    }
}
