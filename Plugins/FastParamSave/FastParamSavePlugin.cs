using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using MissionPlanner;
using MissionPlanner.Controls;
using System.Collections.Generic;
using Microsoft.Scripting.Utils;
using MissionPlanner.Utilities;
using IronPython.Runtime.Operations;

namespace FastParamSave
{
    public class Plugin : MissionPlanner.Plugin.Plugin
    {
		private string missionPlannerPluginDirectory = "C:\\Program Files (x86)\\Mission Planner\\plugins\\";
		//private string fastParamDirectory = "C:\\work\\testFlights\\";
		//private string fastParamParamFileName = "params.param";
		private string fastParamScriptFileName = "fastParamsScript.bat";

		ToolStripMenuItem but;

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

		public override bool Loaded()
        {
			Console.WriteLine("Plugin: FastParam: succesfully loaded FastParmaPlugin");

			but = new ToolStripMenuItem("Fast Param Save");
			but.Click += On_But_Click;
			ToolStripItemCollection col = Host.FDMenuMap.Items;
			col.Add(but);

			// loading settings
			if(!Settings.Instance.ContainsKey("FastParamSaveWorkingDir"))
			{
				Settings.Instance["FastParamSaveWorkingDir"] = "C:\\work\\testFlights\\";
			}
			if (!Settings.Instance.ContainsKey("FastParamSaveFileName"))
			{
				Settings.Instance["FastParamSaveFileName"] = "params.param";
			}

			return true;
        }

        public override bool Loop()
        {
			return true;
        }

        public override bool Exit()
        {
            return true;
        }

		private bool Instance_ProcessCmdKeyCallback(ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData)
		{
			if (keyData == (Keys.Control | Keys.Alt | Keys.P))
			{
				Read_And_Save_Params();
				return true;
			}
			else  if (keyData == (Keys.Control | Keys.Alt | Keys.W))
			{
				string new_working_dir = Settings.Instance["FastParamSaveWorkingDir"];
				if (InputBox.Show("Enter new path for params", "Enter new path for params", ref new_working_dir) == DialogResult.OK)
				{
					new_working_dir = new_working_dir.strip();
					if (new_working_dir[new_working_dir.Length - 1] != '\\')
						new_working_dir += "\\";

					Settings.Instance["FastParamSaveWorkingDir"] = new_working_dir;
				}
				return true;
			}
			else if (keyData == (Keys.Control | Keys.Alt | Keys.N))
			{
				string new_param_file_name = Settings.Instance["FastParamSaveFileName"];
				if (InputBox.Show("Enter new param file name", "Enter new param file name", ref new_param_file_name) == DialogResult.OK)
				{
					new_param_file_name = new_param_file_name.strip();
					Settings.Instance["FastParamSaveFileName"] = new_param_file_name;
				}
				return true;
			}

			return false;
		}

		private void Read_And_Save_Params()
		{
			string git_msg = "";
			if (InputBox.Show("Enter git Messge", "Enter git Messge", ref git_msg) == DialogResult.OK)
			{
				//Console.WriteLine("Plugin FastParamSave: git messge: " + git_msg);

				try
				{
					StreamWriter sw = new StreamWriter(Settings.Instance["FastParamSaveWorkingDir"] + Settings.Instance["FastParamSaveFileName"]);
					SortedDictionary<string, string> paramList = new SortedDictionary<string, string>();
					foreach (string paramName in MissionPlanner.MainV2.comPort.MAV.param.Keys)
					{
						string paramValue = MissionPlanner.MainV2.comPort.MAV.param[paramName].ToString();

						paramList.Add(paramName, paramValue);
					}

					foreach (KeyValuePair<string, string> parameter in paramList)
					{
						sw.WriteLine(parameter.Key + ',' + parameter.Value);
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
				// run cmd command for git commit
				Process p = new Process();
				p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
				p.StartInfo.FileName = "C:\\Windows\\system32\\cmd.exe";
				p.StartInfo.WorkingDirectory = missionPlannerPluginDirectory;
				p.StartInfo.Arguments = "/C " + fastParamScriptFileName + " \"" + Settings.Instance["FastParamSaveWorkingDir"] + "\"" + " \"" + git_msg + "\""; // the /C means execute the following command
				p.Start();
			}
			else
			{
				Console.WriteLine("Plugin FastParamSave: saving params cancled");
			}
		}

		void On_But_Click(object sender, EventArgs e)
		{
			Read_And_Save_Params();
		}
	}
}
