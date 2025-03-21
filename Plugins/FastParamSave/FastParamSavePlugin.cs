using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Utilities;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using MissionPlanner;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace FastParamSave
{
    public class Plugin : MissionPlanner.Plugin.Plugin
    {
       
        public override string Name
        {
            get { return "FastParamSave"; }
        }

        public override string Version
        {
            get { return "0.1"; }
        }

        public override string Author
        {
            get { return "Jair F."; }
        }

        //[DebuggerHidden]
        public override bool Init()
        {
			MainV2.instance.ProcessCmdKeyCallback += this.Instance_ProcessCmdKeyCallback;
			return true;
        }

        private bool Instance_ProcessCmdKeyCallback(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Alt | Keys.P))
			{
                Read_And_Save_Params();

                // run cmd command for git commit
				Process p = new Process();
				p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
				p.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
				p.StartInfo.WorkingDirectory = @"C:\work";
				p.StartInfo.Arguments = "/C C:\\work\\fastParamsScript.bat " + DateTime.Now.ToString(); // the /C means execute the following command
				p.Start();

				return true;
			}

			return false;
		}

        private void Read_And_Save_Params()
        {
			try
			{
				StreamWriter sw = new StreamWriter("C:\\work\\paramOutFile.param");
				foreach (string paramName in MissionPlanner.MainV2.comPort.MAV.param.Keys)
				{
                    //Console.Write("Plugin FastParamSave: paramName: " + paramName);
					var paramValue = MissionPlanner.MainV2.comPort.MAV.param[paramName];
                    //Console.WriteLine(", value: " + paramValue.ToString());

					sw.WriteLine(paramName + ',' + paramValue.ToString());
				}
				sw.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Plugin FastParamSave: Exception: cant write to param out file");
			}
			finally
			{
				Console.WriteLine("Plugin: FastParamSave: written params to file");
			}
		}

		public override bool Loaded()
        {
            Console.WriteLine("Plugin: FastParam: succesfully loaded FastParmaPlugin");
            return true;
        }

        public override bool Loop()
        {
            /*
            MainV2.comPort.MAV.param
            MainV2.comPort.GetParam
            */

			return true;
        }

        public override bool Exit()
        {
            return true;
        }
    }
}
