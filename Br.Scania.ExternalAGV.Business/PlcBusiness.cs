using Br.Scania.ExternalAGV.Model;
using S7.Net;
using System;

namespace Br.Scania.ExternalAGV.Business
{
    public class PlcBusiness
    {
        string IP;
        NlogBusiness nlogBusiness;

        public PlcBusiness()
        {
            IP = "192.168.10.10";
            nlogBusiness = new NlogBusiness();
        }


        public Commands2PLCModel ReadCommandsPLC()
        {
            try
            {
                nlogBusiness.Write("Initialize Plc Rotine");
                using (var plc = new Plc(CpuType.S7300, IP, 0, 2))
                {
                    nlogBusiness.Write("Testing if Plc is available");
                    if (plc.IsAvailable)
                    {
                        nlogBusiness.Write("Plc is available!");
                        plc.Close();
                        plc.Open();
                        nlogBusiness.Write("Testing if Plc is connected");
                        if (plc.IsConnected)
                        {
                            nlogBusiness.Write("Plc is connected!");
                            Commands2PLCModel commands2PLCModel = new Commands2PLCModel();
                            nlogBusiness.Write("Writting in PLC!");
                            int ret = plc.ReadClass(commands2PLCModel, 30, 0);
                            if (ret == 0)
                            {
                                nlogBusiness.Write("PLC writing successfully");
                            }
                            else
                            {
                                nlogBusiness.Write("Fault PLC writing");
                            }
                            plc.Close();
                            nlogBusiness.Write("Plc is disconnected!");
                            return commands2PLCModel;
                        }
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                nlogBusiness.Error(ex, "Error ReadCommandsPLC");
                return null;
            }
            finally
            {
                nlogBusiness.Write("Finalize Plc Rotine");
            }
        }

        public bool WriteCommandsPLC(Commands2PLCModel commands)
        {
            try
            {
                nlogBusiness.Write("Initialize Plc Rotine");
                using (var plc = new Plc(CpuType.S7300, IP, 0, 2))
                {
                    nlogBusiness.Write("Testing if Plc is available");
                    if (plc.IsAvailable)
                    {
                        nlogBusiness.Write("Plc is available!");
                        plc.Close();
                        plc.Open();
                        nlogBusiness.Write("Testing if Plc is connected");
                        if (plc.IsConnected)
                        {
                            nlogBusiness.Write("Writting in PLC!");
                            plc.WriteClassAsync(commands, 30, 0);
                            plc.Close();
                            nlogBusiness.Write("Plc is disconnected!");
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                nlogBusiness.Error(ex, "Error ReadCommandsPLC");
                return false;
            }
            finally
            {
                nlogBusiness.Write("Finalize Plc Rotine");
            }

        }

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        public Parameters2PLCModel ReadParametersPLC()
        {
            try
            {
                nlogBusiness.Write("Initialize Plc Rotine");
                using (var plc = new Plc(CpuType.S7300, IP, 0, 2))
                {
                    nlogBusiness.Write("Testing if Plc is available");
                    if (plc.IsAvailable)
                    {
                        nlogBusiness.Write("Plc is available!");
                        plc.Close();
                        plc.Open();
                        nlogBusiness.Write("Testing if Plc is connected");
                        if (plc.IsConnected)
                        {
                            Parameters2PLCModel paramters2PLCModel = new Parameters2PLCModel();
                            nlogBusiness.Write("Writting in PLC!");
                            int ret = plc.ReadClass(paramters2PLCModel, 33, 0);
                            if (ret == 0)
                            {
                                nlogBusiness.Write("PLC writing successfully");
                            }
                            else
                            {
                                nlogBusiness.Write("Fault PLC writing");
                            }
                            plc.Close();
                            nlogBusiness.Write("Plc is disconnected!");
                            return paramters2PLCModel;
                        }
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                nlogBusiness.Error(ex, "Error ReadCommandsPLC");
                return null;
            }
            finally
            {
                nlogBusiness.Write("Finalize Plc Rotine");
            }
        }

        public bool WriteCommandsPLC(Parameters2PLCModel parameters)
        {
            try
            {
                nlogBusiness.Write("Initialize Plc Rotine");
                using (var plc = new Plc(CpuType.S7300, IP, 0, 2))
                {
                    nlogBusiness.Write("Testing if Plc is available");
                    if (plc.IsAvailable)
                    {
                        nlogBusiness.Write("Plc is available!");
                        plc.Close();
                        plc.Open();
                        nlogBusiness.Write("Testing if Plc is connected");
                        if (plc.IsConnected)
                        {
                            nlogBusiness.Write("Writting in PLC!");
                            plc.WriteClassAsync(parameters, 33, 0);
                            plc.Close();
                            nlogBusiness.Write("Plc is disconnected!");
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                nlogBusiness.Error(ex, "Error ReadCommandsPLC");
                return false;
            }
            finally
            {
                nlogBusiness.Write("Finalize Plc Rotine");
            }

        }

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        public StatusPLCModel ReadStatusPLC()
        {
            try
            {
                //nlogBusiness.Write("Initialize Plc Rotine");
                using (var plc = new Plc(CpuType.S7300, IP, 0, 2))
                {
                    //nlogBusiness.Write("Testing if Plc is available");
                    if (plc.IsAvailable)
                    {
                        nlogBusiness.Write("Plc is available!");
                        plc.Close();
                        plc.Open();
                        //nlogBusiness.Write("Testing if Plc is connected");
                        if (plc.IsConnected)
                        {
                            StatusPLCModel statusPLCModel = new StatusPLCModel();
                            //nlogBusiness.Write("Writting in PLC!");
                            int ret = plc.ReadClass(statusPLCModel, 26, 0);
                            if (ret == 0)
                            {
                                nlogBusiness.Write("PLC writing successfully");
                            }
                            else
                            {
                                nlogBusiness.Write("Fault PLC writing");
                            }
                            plc.Close();
                            //nlogBusiness.Write("Plc is disconnected!");
                            return statusPLCModel;
                        }
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                nlogBusiness.Error(ex, "Error ReadCommandsPLC");
                return null;
            }
            finally
            {
                nlogBusiness.Write("Finalize Plc Rotine");
            }
        }

        ~PlcBusiness()  // finalizer
        {
            nlogBusiness = null;
        }
    }
}
