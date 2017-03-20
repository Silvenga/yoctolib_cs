/*********************************************************************
 *
 * $Id: yocto_poweroutput.cs 26751 2017-03-14 08:04:50Z seb $
 *
 * Implements yFindPowerOutput(), the high-level API for PowerOutput functions
 *
 * - - - - - - - - - License information: - - - - - - - - - 
 *
 *  Copyright (C) 2011 and beyond by Yoctopuce Sarl, Switzerland.
 *
 *  Yoctopuce Sarl (hereafter Licensor) grants to you a perpetual
 *  non-exclusive license to use, modify, copy and integrate this
 *  file into your software for the sole purpose of interfacing
 *  with Yoctopuce products.
 *
 *  You may reproduce and distribute copies of this file in
 *  source or object form, as long as the sole purpose of this
 *  code is to interface with Yoctopuce products. You must retain
 *  this notice in the distributed source file.
 *
 *  You should refer to Yoctopuce General Terms and Conditions
 *  for additional information regarding your rights and
 *  obligations.
 *
 *  THE SOFTWARE AND DOCUMENTATION ARE PROVIDED 'AS IS' WITHOUT
 *  WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING 
 *  WITHOUT LIMITATION, ANY WARRANTY OF MERCHANTABILITY, FITNESS
 *  FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO
 *  EVENT SHALL LICENSOR BE LIABLE FOR ANY INCIDENTAL, SPECIAL,
 *  INDIRECT OR CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA,
 *  COST OF PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY OR
 *  SERVICES, ANY CLAIMS BY THIRD PARTIES (INCLUDING BUT NOT
 *  LIMITED TO ANY DEFENSE THEREOF), ANY CLAIMS FOR INDEMNITY OR
 *  CONTRIBUTION, OR OTHER SIMILAR COSTS, WHETHER ASSERTED ON THE
 *  BASIS OF CONTRACT, TORT (INCLUDING NEGLIGENCE), BREACH OF
 *  WARRANTY, OR OTHERWISE.
 *
 *********************************************************************/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using YDEV_DESCR = System.Int32;
using YFUN_DESCR = System.Int32;

    //--- (YPowerOutput return codes)
    //--- (end of YPowerOutput return codes)
//--- (YPowerOutput dlldef)
//--- (end of YPowerOutput dlldef)
//--- (YPowerOutput class start)
/**
 * <summary>
 *   Yoctopuce application programming interface allows you to control
 *   the power ouput featured on some devices such as the Yocto-Serial.
 * <para>
 * </para>
 * <para>
 * </para>
 * </summary>
 */
public class YPowerOutput : YFunction
{
//--- (end of YPowerOutput class start)
    //--- (YPowerOutput definitions)
    public new delegate void ValueCallback(YPowerOutput func, string value);
    public new delegate void TimedReportCallback(YPowerOutput func, YMeasure measure);

    public const int VOLTAGE_OFF = 0;
    public const int VOLTAGE_OUT3V3 = 1;
    public const int VOLTAGE_OUT5V = 2;
    public const int VOLTAGE_INVALID = -1;
    protected int _voltage = VOLTAGE_INVALID;
    protected ValueCallback _valueCallbackPowerOutput = null;
    //--- (end of YPowerOutput definitions)

    public YPowerOutput(string func)
        : base(func)
    {
        _className = "PowerOutput";
        //--- (YPowerOutput attributes initialization)
        //--- (end of YPowerOutput attributes initialization)
    }

    //--- (YPowerOutput implementation)

    protected override void _parseAttr(YAPI.TJSONRECORD member)
    {
        if (member.name == "voltage")
        {
            _voltage = (int)member.ivalue;
            return;
        }
        base._parseAttr(member);
    }

    /**
     * <summary>
     *   Returns the voltage on the power ouput featured by
     *   the module.
     * <para>
     * </para>
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a value among <c>YPowerOutput.VOLTAGE_OFF</c>, <c>YPowerOutput.VOLTAGE_OUT3V3</c> and
     *   <c>YPowerOutput.VOLTAGE_OUT5V</c> corresponding to the voltage on the power ouput featured by
     *   the module
     * </returns>
     * <para>
     *   On failure, throws an exception or returns <c>YPowerOutput.VOLTAGE_INVALID</c>.
     * </para>
     */
    public int get_voltage()
    {
        int res;
        lock (thisLock) {
            if (this._cacheExpiration <= YAPI.GetTickCount()) {
                if (this.load(YAPI.DefaultCacheValidity) != YAPI.SUCCESS) {
                    return VOLTAGE_INVALID;
                }
            }
            res = this._voltage;
        }
        return res;
    }

    /**
     * <summary>
     *   Changes the voltage on the power output provided by the
     *   module.
     * <para>
     *   Remember to call the <c>saveToFlash()</c> method of the module if the
     *   modification must be kept.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="newval">
     *   a value among <c>YPowerOutput.VOLTAGE_OFF</c>, <c>YPowerOutput.VOLTAGE_OUT3V3</c> and
     *   <c>YPowerOutput.VOLTAGE_OUT5V</c> corresponding to the voltage on the power output provided by the
     *   module
     * </param>
     * <para>
     * </para>
     * <returns>
     *   <c>YAPI.SUCCESS</c> if the call succeeds.
     * </returns>
     * <para>
     *   On failure, throws an exception or returns a negative error code.
     * </para>
     */
    public int set_voltage(int newval)
    {
        string rest_val;
        rest_val = (newval).ToString();
        return _setAttr("voltage", rest_val);
    }

    /**
     * <summary>
     *   Retrieves a dual power  ouput control for a given identifier.
     * <para>
     *   The identifier can be specified using several formats:
     * </para>
     * <para>
     * </para>
     * <para>
     *   - FunctionLogicalName
     * </para>
     * <para>
     *   - ModuleSerialNumber.FunctionIdentifier
     * </para>
     * <para>
     *   - ModuleSerialNumber.FunctionLogicalName
     * </para>
     * <para>
     *   - ModuleLogicalName.FunctionIdentifier
     * </para>
     * <para>
     *   - ModuleLogicalName.FunctionLogicalName
     * </para>
     * <para>
     * </para>
     * <para>
     *   This function does not require that the power ouput control is online at the time
     *   it is invoked. The returned object is nevertheless valid.
     *   Use the method <c>YPowerOutput.isOnline()</c> to test if the power ouput control is
     *   indeed online at a given time. In case of ambiguity when looking for
     *   a dual power  ouput control by logical name, no error is notified: the first instance
     *   found is returned. The search is performed first by hardware name,
     *   then by logical name.
     * </para>
     * </summary>
     * <param name="func">
     *   a string that uniquely characterizes the power ouput control
     * </param>
     * <returns>
     *   a <c>YPowerOutput</c> object allowing you to drive the power ouput control.
     * </returns>
     */
    public static YPowerOutput FindPowerOutput(string func)
    {
        YPowerOutput obj;
        lock (YAPI.globalLock) {
            obj = (YPowerOutput) YFunction._FindFromCache("PowerOutput", func);
            if (obj == null) {
                obj = new YPowerOutput(func);
                YFunction._AddToCache("PowerOutput", func, obj);
            }
        }
        return obj;
    }

    /**
     * <summary>
     *   Registers the callback function that is invoked on every change of advertised value.
     * <para>
     *   The callback is invoked only during the execution of <c>ySleep</c> or <c>yHandleEvents</c>.
     *   This provides control over the time when the callback is triggered. For good responsiveness, remember to call
     *   one of these two functions periodically. To unregister a callback, pass a null pointer as argument.
     * </para>
     * <para>
     * </para>
     * </summary>
     * <param name="callback">
     *   the callback function to call, or a null pointer. The callback function should take two
     *   arguments: the function object of which the value has changed, and the character string describing
     *   the new advertised value.
     * @noreturn
     * </param>
     */
    public int registerValueCallback(ValueCallback callback)
    {
        string val;
        if (callback != null) {
            YFunction._UpdateValueCallbackList(this, true);
        } else {
            YFunction._UpdateValueCallbackList(this, false);
        }
        this._valueCallbackPowerOutput = callback;
        // Immediately invoke value callback with current value
        if (callback != null && this.isOnline()) {
            val = this._advertisedValue;
            if (!(val == "")) {
                this._invokeValueCallback(val);
            }
        }
        return 0;
    }

    public override int _invokeValueCallback(string value)
    {
        if (this._valueCallbackPowerOutput != null) {
            this._valueCallbackPowerOutput(this, value);
        } else {
            base._invokeValueCallback(value);
        }
        return 0;
    }

    /**
     * <summary>
     *   Continues the enumeration of dual power ouput controls started using <c>yFirstPowerOutput()</c>.
     * <para>
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YPowerOutput</c> object, corresponding to
     *   a dual power  ouput control currently online, or a <c>null</c> pointer
     *   if there are no more dual power ouput controls to enumerate.
     * </returns>
     */
    public YPowerOutput nextPowerOutput()
    {
        string hwid = "";
        if (YAPI.YISERR(_nextFunction(ref hwid)))
            return null;
        if (hwid == "")
            return null;
        return FindPowerOutput(hwid);
    }

    //--- (end of YPowerOutput implementation)

    //--- (PowerOutput functions)

    /**
     * <summary>
     *   Starts the enumeration of dual power ouput controls currently accessible.
     * <para>
     *   Use the method <c>YPowerOutput.nextPowerOutput()</c> to iterate on
     *   next dual power ouput controls.
     * </para>
     * </summary>
     * <returns>
     *   a pointer to a <c>YPowerOutput</c> object, corresponding to
     *   the first dual power ouput control currently online, or a <c>null</c> pointer
     *   if there are none.
     * </returns>
     */
    public static YPowerOutput FirstPowerOutput()
    {
        YFUN_DESCR[] v_fundescr = new YFUN_DESCR[1];
        YDEV_DESCR dev = default(YDEV_DESCR);
        int neededsize = 0;
        int err = 0;
        string serial = null;
        string funcId = null;
        string funcName = null;
        string funcVal = null;
        string errmsg = "";
        int size = Marshal.SizeOf(v_fundescr[0]);
        IntPtr p = Marshal.AllocHGlobal(Marshal.SizeOf(v_fundescr[0]));
        err = YAPI.apiGetFunctionsByClass("PowerOutput", 0, p, size, ref neededsize, ref errmsg);
        Marshal.Copy(p, v_fundescr, 0, 1);
        Marshal.FreeHGlobal(p);
        if ((YAPI.YISERR(err) | (neededsize == 0)))
            return null;
        serial = "";
        funcId = "";
        funcName = "";
        funcVal = "";
        errmsg = "";
        if ((YAPI.YISERR(YAPI.yapiGetFunctionInfo(v_fundescr[0], ref dev, ref serial, ref funcId, ref funcName, ref funcVal, ref errmsg))))
            return null;
        return FindPowerOutput(serial + "." + funcId);
    }



    //--- (end of PowerOutput functions)
}
