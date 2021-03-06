﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
  class Program
  {
    static void usage()
    {
      string execname = System.AppDomain.CurrentDomain.FriendlyName;
      Console.WriteLine("Usage");
      Console.WriteLine(execname + " <serial_number> <value>");
      Console.WriteLine(execname + " <logical_name>  <value>");
      Console.WriteLine(execname +
                        " any  <value>    (use any discovered device)");
      Console.WriteLine("     <value>: floating point number between 0.0 and 10.000");
      System.Threading.Thread.Sleep(2500);
      Environment.Exit(0);
    }

    static void Main(string[] args)
    {
      string errmsg = "";
      string target;
      YVoltageOutput vout1;
      YVoltageOutput vout2;
      double voltage;

      if (args.Length < 2) usage();
      target = args[0].ToUpper();
      voltage = Convert.ToDouble(args[1]);

      if (YAPI.RegisterHub("usb", ref errmsg) != YAPI.SUCCESS) {
        Console.WriteLine("RegisterHub error: " + errmsg);
        Environment.Exit(0);
      }

      if (target == "ANY") {
        vout1 = YVoltageOutput.FirstVoltageOutput();
        if (vout1 == null) {
          Console.WriteLine("No module connected (check USB cable) ");
          Environment.Exit(0);
        }
        target = vout1.get_module().get_serialNumber();
      }

      vout1 = YVoltageOutput.FindVoltageOutput(target + ".voltageOutput1");
      vout2 = YVoltageOutput.FindVoltageOutput(target + ".voltageOutput2");

      if (vout1.isOnline()) {
        // output 1 : immediate change
        vout1.set_currentVoltage(voltage);
        // output 2 : smooth change
        vout2.voltageMove(voltage, 3000);
      } else {
        Console.WriteLine("Module not connected (check identification and USB cable)");
      }
      YAPI.FreeAPI();
    }
  }
}
