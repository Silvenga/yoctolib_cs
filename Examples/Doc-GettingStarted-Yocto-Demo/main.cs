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
      string  execname  = System.AppDomain.CurrentDomain.FriendlyName;
      Console.WriteLine(execname+" <serial_number>  [ on | off ]");
      Console.WriteLine(execname+" <logical_name> [ on | off ]");
      Console.WriteLine(execname+" any [ on | off ] ");
      System.Threading.Thread.Sleep(2500);
      Environment.Exit(0);
    }

    static void Main(string[] args)
    {
      string errmsg = "";
      string target;
      YLed led;
      string on_off;

      if (args.Length < 2) usage();
      target = args[0].ToUpper();
      on_off = args[1].ToUpper();

      if (YAPI.RegisterHub("usb", ref errmsg) != YAPI.SUCCESS)
      {
        Console.WriteLine("RegisterHub error: " + errmsg);
        Environment.Exit(0);
      }

      if (target == "ANY")
      {
        led = YLed.FirstLed();
        if (led == null)
        {
          Console.WriteLine("No module connected (check USB cable) ");
          Environment.Exit(0);
        }
      }
      else led = YLed.FindLed(target + ".led");

      if (led.isOnline())
      {
        if (on_off == "ON") led.set_power(YLed.POWER_ON); else led.set_power(YLed.POWER_OFF);
      }
      else Console.WriteLine("Module not connected (check identification and USB cable)");
    }
  }
}
