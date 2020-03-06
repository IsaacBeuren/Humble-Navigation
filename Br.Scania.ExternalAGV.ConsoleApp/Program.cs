using System;
using Br.Scania.ExternalAGV.Model;
using Br.Scania.ExternalAGV.Business;
using Br.Scania.ExternalAGV.Model.DataBase;
using CoordinateSharp;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.IO.Ports;
using System.Diagnostics;
using System.Text;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Specialized;

namespace Br.Scania.ExternalAGV.ConsoleApp
{
    class Program
    {
        static int AGVID = Convert.ToInt16(1);
        static LastPositionBusiness lastPositionBusiness = new LastPositionBusiness();
        static CoordinatesBusiness coordinatesBusiness = new CoordinatesBusiness();
        static PointsBusiness points = new PointsBusiness();
        static Coordinate last_but_one;
        static bool Stopped;
        static bool MoreThanOne;
        static PointsModel nextPoint = new PointsModel();
        static NmeaBusiness nmea = new NmeaBusiness();
        static PlcBusiness plc = new PlcBusiness();
        static FilterBusiness filter = new FilterBusiness();
        static bool Write2PLC = false;
        static NlogBusiness nlog = new NlogBusiness();
        static PointsModel pointModel;

        static ConfigPointsBusiness configPointsBusiness = new ConfigPointsBusiness();
        static int counter = 0;
        static StringBuilder csv = new StringBuilder();
        static StringBuilder csv2 = new StringBuilder();
        static int route;
        static string pickUpPoint;
        static string dropPoint;
        static ConfigBusiness configBusiness = new ConfigBusiness();
        static bool run = false;
        static bool button = false;
        static string newLine = "";
        static int rest = 0;
        static int pointCounter = 0;

        static CallsBusiness callsBusiness = new CallsBusiness();
        static CallsModel callsModel = new CallsModel();
        static RouteBusiness routeBusiness = new RouteBusiness();
        static RouteModel routeModel = new RouteModel();
        static List<PointsModel> pointList = new List<PointsModel>();
        static string pointsString;
        static string routeString;
        static bool reset = false;
        static string time1 = "0";
        static string time2 = "0";
        static string time3 = "0";
        static string time4 = "0";
        static string timeTotal = "";
        static double distance = 999;
        static bool pointSwitch = false;
        static string key;
        static bool lostThePoint = false;
        static bool buttonBool = false;
        static int buttonCounter = 0;

        private static readonly HttpClient client = new HttpClient();
        

        static void Main(string[] args)
        {
            try
            {

                //Console: ponto - qualidade da antenna - processamento - temperatura contCONSOLE
                //PLC: anguloREAL - velocidadeREAL - contPLC
                //começa a contar a partir do run

                //nlog.Write("Reading appsettings.json");


                var builder = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                IConfigurationRoot configuration = builder.Build();

                int AGVID = Convert.ToInt16(configuration.GetSection("AGV").GetSection("ID").Value);
                string Token = configuration.GetSection("Token").GetSection("ID").Value;
                Write2PLC = Convert.ToBoolean(configuration.GetSection("PLC").GetSection("Write").Value);
                int SerialPort = Convert.ToInt16(configuration.GetSection("GPS").GetSection("SerialPort").Value);

                nlog.Write("Write PLC =" + Write2PLC + " AGVID: " + AGVID);
                

                
                //Coordinate idealCoordinate = new Coordinate();
                //idealCoordinate = coordinatesBusiness.DegreeDecimalMinute2Coordinate(-2342.72985134912,-4634.01152534427);
                //Coordinate coordinateActual = new Coordinate();
                //coordinateActual = coordinatesBusiness.DegreeDecimalMinute2Coordinate(-2342.72317091554,-4634.0117949375);
                //Distance pathDiviation = coordinatesBusiness.Calculating_Distance(idealCoordinate, coordinateActual);
                //Console.Write("PathDeviation: " + pathDiviation.Meters);

                //Wait the CLP signal to start
                while (true)
                {
                    var statusStart = plc.ReadStatusPLC();
                    if (statusStart.BT_Load) { break; }

                }


                //Console.WriteLine("\nAWS (1)  Local (2)");

                //key = Console.ReadKey().Key.ToString();

                //if (key == "D1")
                //{
                //    try
                //    {
                //        points.RemoveAll();
                //        pointsString = new WebClient().DownloadString("http://agv-api.eu-west-1.elasticbeanstalk.com/api/Points/GetAll");
                //        pointList = JsonConvert.DeserializeObject<List<PointsModel>>(pointsString);
                //        foreach (var point in pointList) { points.Insert(point); }
                //        routeString = new WebClient().DownloadString("http://agv-api.eu-west-1.elasticbeanstalk.com/api/Route/GetAGVByID?ID=" + pointList[0].IDRoute);
                //        pickUpPoint = JsonConvert.DeserializeObject<RouteModel>(routeString).PickUpPoint.Replace(" ", "");
                //        dropPoint = JsonConvert.DeserializeObject<RouteModel>(routeString).DropPoint.Replace(" ", ""); ;
                //    }
                //    catch
                //    {
                //        Console.Clear();
                //    }
                //}
                //else
                //{

                    //It Resets the route on the DB and then copies it
                    points.ResetRoute();
                    pointsString = new WebClient().DownloadString("http://localhost/agvAPI/api/Points/GetAll");
                    pointList = JsonConvert.DeserializeObject<List<PointsModel>>(pointsString);
                    route = pointList[0].IDRoute; 

                    pickUpPoint = "PICKUP_EIXO1";
                    dropPoint = "DROP_EIXO";
                //}

                foreach (var point in pointList)
                {
                    if (point.Done == true) { pointCounter++; }
                    else { break; }
                }

                //Inicializating the requirements to send data to the control tower
                routeModel.ID = 7035;
                routeModel.Description = "ALERTROUTE";
                routeModel.DropPoint = "";
                routeModel.PickUpPoint = "";
                

                //It defines the first point(coordinate) to follow
                try
                {
                    nextPoint = pointList[pointCounter];
                    Console.WriteLine("\nNextPoint: " + nextPoint.Description);
                }
                catch
                {
                    nlog.Write("Rota completada");
                    nextPoint = null;
                    Console.WriteLine("NextPoint: null");
                }
                
                try
                {
                    SerialPortBusiness serialPort = new SerialPortBusiness();

                    bool portOK = serialPort.CheckSerialPort("COM" + SerialPort);

                    if (portOK)
                    {
                        nlog.Write("Serial port was found");
                        SerialPort mySerialPort = new SerialPort("COM" + SerialPort);

                        mySerialPort.BaudRate = Convert.ToInt32(configuration.GetSection("GPS").GetSection("BaudRate").Value);
                        mySerialPort.Parity = (Parity)Enum.Parse(typeof(Parity), configuration.GetSection("GPS").GetSection("Parity").Value);
                        mySerialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), configuration.GetSection("GPS").GetSection("StopBits").Value);
                        mySerialPort.DataBits = Convert.ToInt16(configuration.GetSection("GPS").GetSection("DataBits").Value);
                        mySerialPort.Handshake = (Handshake)Enum.Parse(typeof(Handshake), configuration.GetSection("GPS").GetSection("Handshake").Value);
                        mySerialPort.RtsEnable = Convert.ToBoolean(configuration.GetSection("GPS").GetSection("RtsEnable").Value);

                        mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                        //nlog.Write("Trying opening Serial port");
                        mySerialPort.Open();
                        Console.WriteLine("\n\nWaiting for GPS data...");

                        Console.ReadKey();
                        mySerialPort.Close();
                    }
                    else
                    {
                        nlog.Write("Serial Port not found");
                    }
                }
                catch (Exception ex)
                {
                    // NLog: catch any exception and log it.
                    nlog.Error(ex, "1");
                    Console.WriteLine("Serial Port Fault...");
                }
                finally
                {
                    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                    nlog.Shutdown();
                }
                
            }
            catch (Exception ex)
            {
                // NLog: catch any exception and log it.
                nlog.Error(ex, "2");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                nlog.Shutdown();
            }

        }

        static bool isBusy = false;
        static string msgAscII = "";
        static Coordinate idealCoordinate = new Coordinate();
        static Distance pathDiviation;
        static Coordinate coordinateDestination = new Coordinate();
        static Distance distance2Destination;
        static Commands2PLCModel commands = new Commands2PLCModel();
        static Distance course;
        static Coordinate coordinateLastest = new Coordinate();
        static Coordinate coordinateActual = new Coordinate();
        static LastPositionModel lastPosition = new LastPositionModel();
        static GGAModel gga;
        static GGAModel ggaModel;
        static Stopwatch stopwatch1 = new Stopwatch();
        static Stopwatch stopwatch2 = new Stopwatch();
        static Stopwatch stopwatch3 = new Stopwatch();
        static DateTime utcDate;

        static double a1;
        static double b1;
        static double a2;
        static double b2;
        static double xi;
        static double yi;
        static int side;
        static double angle;
        static double dockage;

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            //string that contains the timing of the last handler
            timeTotal = "T1: " + time1 + "ms  T2: " + time2 + "ms  T3: " + time3 + "ms  T4: " + time4 + "ms T:" + (Convert.ToInt32(time1) + Convert.ToInt32(time2) + Convert.ToInt32(time4)).ToString() +"ms";

            nlog.Write(" isBusy: " + isBusy.ToString() + " " + counter.ToString());
            if (isBusy)
            {
                nlog.Write("Retornou");
                return;
            }

            isBusy = true;

            try
            {
                string[] result = null;
                SerialPort sp = (SerialPort)sender;

                if (msgAscII.Length < 88)
                {
                    msgAscII = msgAscII + sp.ReadExisting();
                }
                else
                {
                    //Reseting the timing
                    time1 = "0";
                    time2 = "0";
                    time3 = "0";

                    result = msgAscII.Split("$G");
                    msgAscII = "";
                    NMEAModel nmea = new NMEAModel();
                    NmeaBusiness nmeaBusiness = new NmeaBusiness();

                    foreach (string item in result)
                    {

                        if (item.Length > 5)
                        {
                            string ret = "";
                            try
                            {
                                ret = item.Substring(1, item.Length - 1);
                            }
                            catch (Exception ex)
                            {
                                nlog.Error(ex, "3");
                            }
                            string Header = ret.Substring(0, 3);

                            if (Header == "GGA")
                            {
                                //starting the fist timing count
                                stopwatch1.Start();

                                //nlog.Write("GGA protocol successfully received");
                                ggaModel = nmeaBusiness.ConvertNmea2GGA(ret);
                                if (ggaModel != null)
                                {
                                    msgAscII = "";
                                }
                                //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

                                gga = filter.ApplyFilter(ggaModel);
                                //nlog.Write("GGA protocol filter successfully applied");

                                //Updating the AMR last position on the DB
                                utcDate = DateTime.UtcNow;
                                lastPosition.IDAGV = AGVID;
                                lastPosition.Latitude = Convert.ToDouble(gga.Latitude);
                                lastPosition.Longitude = Convert.ToDouble(gga.Longitude);
                                lastPosition.UpdateTime = utcDate;
                                lastPosition.GPSQuality = Convert.ToInt32(gga.GPS_Quality_indicator);
                                lastPositionBusiness.Update(lastPosition);
                                //nlog.Write("Last coordinates successfully update");

                                //Actual GPS Position 
                                nlog.Write("Getting actual/last position, calculating course to upadate ");
                                coordinateActual = coordinatesBusiness.DegreeDecimalMinute2Coordinate(gga.Latitude, gga.Longitude);

                                //Last GPS Position
                                coordinateLastest = last_but_one;

                                course = coordinatesBusiness.Calculating_Distance(coordinateActual, coordinateLastest);

                                //Updating last position
                                last_but_one = coordinateActual;
                                
                                
                                //ending the first timing count
                                stopwatch1.Stop();
                                nlog.Write("TIME1: " + stopwatch1.ElapsedMilliseconds);
                                time1 = stopwatch1.ElapsedMilliseconds.ToString();

                                stopwatch1.Reset();




                                if (nextPoint != null && nextPoint.Lat != 0 && course != null && reset == false)
                                {

                                    //starting the second timing count
                                    stopwatch2.Start();


                                    //Converting actual coordinates
                                    coordinateDestination = coordinatesBusiness.DegreeDecimalMinute2Coordinate(nextPoint.Lat, nextPoint.Lng);

                                    //Calculating angle between actual point and destination
                                    distance2Destination = coordinatesBusiness.Calculating_Distance(coordinateDestination, coordinateActual);


                                    try
                                    {
                                        pointModel = pointList[pointCounter - 1]; //getting the actual point 
                                    }
                                    catch { pointModel = null; }


                                    //Reding the PLC
                                    var status = plc.ReadStatusPLC();
                                    
                                    
                                    //Sending the PLC information to the DB
                                    routeModel.Routes = status.LiddarScanner + ";" + status.LeftScanner + ";" + status.RightScanner + ";" + status.VehicleRun + ";" + status.VelocityTraction + ";";
                                    routeBusiness.UpdateRoute(routeModel);

                                    if (pointModel != null)  //Verifying if the vehile is searcging for the first point
                                    {


                                        try
                                        {

                                            if (pointModel.Description == pickUpPoint || pointModel.Description == dropPoint)
                                            {
                                                nlog.Write(" StopPoint " + pointModel.Description);
                                                run = false; //stop the vehile if it arrives at the pick up coordenates
                                                if (status.BT_Load == true && pointModel.Description == pickUpPoint)
                                                {
                                                    nlog.Write(" ButtonLoad True ");
                                                    button = true; //the release button on the PLC was pressed
                                                }
                                            }
                                            else
                                            {
                                                run = true;
                                                button = false;
                                            }

                                            if (button == true) { run = true;} //if the button was pressed on the pick up point, the AMR proceeds the route

                                            //Collecting data to send to the PLC
                                            commands.EnableAuto = Convert.ToBoolean(run);
                                            commands.LeftLight = Convert.ToBoolean(pointModel.LeftLight);
                                            commands.RightLight = Convert.ToBoolean(pointModel.RightLight);
                                            commands.Velocity = Convert.ToInt16(pointModel.Velocity);
                                            commands.OnStraight = Convert.ToBoolean(pointModel.OnStraight);
                                            commands.Counter = Convert.ToInt16(counter);

                                        }
                                        catch (Exception ex)
                                        {
                                            nlog.Error(ex, "DB error ");
                                        }


                                        //Calculation the Path Deviation
                                        //double x1 = pointModel.Lng;
                                        //double x2 = nextPoint.Lng;
                                        //double y1 = pointModel.Lat;
                                        //double y2 = nextPoint.Lat;
                                        a1 = (nextPoint.Lat - pointModel.Lat) / (nextPoint.Lng - pointModel.Lng);
                                        b1 = pointModel.Lat - (a1 * pointModel.Lng);
                                        a2 = -1/a1;
                                        b2 = lastPosition.Latitude - (a2*lastPosition.Longitude);
                                        xi = (b1 - b2) / (a2 - a1);
                                        yi = (a1 * xi) + b1;


                                        stopwatch3.Start();


                                        idealCoordinate = coordinatesBusiness.DegreeDecimalMinute2Coordinate(yi, xi);
                                        pathDiviation = coordinatesBusiness.Calculating_Distance(idealCoordinate, coordinateActual);


                                        stopwatch3.Stop();
                                        nlog.Write("TIME3: " + stopwatch3.ElapsedMilliseconds);
                                        time3 = stopwatch3.ElapsedMilliseconds.ToString();
                                        stopwatch3.Reset();

                                        if (nextPoint.Lng > pointModel.Lng)
                                        {
                                            if (lastPosition.Latitude < yi) { side = 100; }
                                            else { side = -100;  }
                                        }
                                        else
                                        {
                                            if (lastPosition.Latitude < yi) { side = -100; }
                                            else { side = 100; }
                                        }
                                        commands.PathDeviation = side*pathDiviation.Meters;
                                        Console.WriteLine(" \n -- PathDiviation: " + commands.PathDeviation + "cm\n");



                                    }
                                    else
                                    {
                                        //Collecting data to send to the PLC
                                        nlog.Write("Searching for the route");
                                        commands.EnableAuto = true;
                                        commands.LeftLight = false;
                                        commands.RightLight = false;
                                        commands.Velocity = 2;
                                        commands.OnStraight = false;
                                        commands.Counter = Convert.ToInt16(counter);
                                        commands.PathDeviation = 0;
                                    }

                                    //Getting the angle to send to the PLC
                                    angle = coordinatesBusiness.AngularCalculation(course.Bearing, distance2Destination.Bearing);
                                    commands.RealDegree = Convert.ToInt32(angle * 100);


                                    //try
                                    //{
                                    //    if (distance2Destination.Meters <= 2 && commands.OnStraight == true) //Applying a filter when the AMR is about to switch points and also is on a straight path
                                    //    {
                                    //        //Signals if the AMR left a point behind without passing through it
                                    //        if (distance < distance2Destination.Meters && Math.Abs(distance - distance2Destination.Meters) > 0.01 && Convert.ToBoolean(status.ManualEnabled) == false)
                                    //        {
                                    //            lostThePoint = true;
                                    //        }

                                    //        if (angle > 2 || angle < -2) { angle = 2 * ((angle) / Math.Abs(angle)); } //filter 
                                    //    }

                                    //    if (pointSwitch == true && commands.OnStraight == true) //applying an one-timer filter when the AMR switch points
                                    //    {
                                    //        angle = 0; //filter
                                    //        pointSwitch = false;
                                    //    }

                                    //}
                                    //catch (Exception ex)
                                    //{
                                    //    // NLog: catch any exception and log it.
                                    //    nlog.Error(ex, "Filter error");
                                    //}

                                    distance = distance2Destination.Meters;

                                    if (commands.OnStraight == true) { dockage = 4; }
                                    else { dockage = 1.5; }

                                    try
                                    {
                                        if (distance2Destination.Meters <= dockage || lostThePoint == true) //Switching points when the AMR gets 1.5m close to the point or when it losts the point
                                        {
                                            //nlog.Write("Locking for the next destination");

                                            if (points.PostPointDone(nextPoint)) //Amrk the point as "done" on the DB
                                            {
                                                pointCounter++; //gets the next point
                                                try
                                                {
                                                    nextPoint = pointList[pointCounter]; //points to the next point
                                                    pointSwitch = true;

                                                    string lostPoint = "";
                                                    if (lostThePoint == true)
                                                    {
                                                        lostThePoint = false;
                                                        lostPoint = "   (LOST THE POINT)";

                                                    }
                                                    Console.WriteLine("\n Next point " + nextPoint.Description + ". Now at " + pointList[pointCounter - 1].Velocity.ToString() + "km/h  " + lostPoint);
                                                }
                                                catch { nextPoint = null; }
                                                //nlog.Write("Next destination was found");
                                            }
                                            else
                                            {
                                                commands.EnableAuto = false;
                                                nlog.Write("Next destination not found");
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        // NLog: catch any exception and log it.
                                        nlog.Error(ex, "4");
                                    }

                                    if (course != null)
                                    {
                                        //Console.WriteLine(course.Meters);
                                        if (course.Meters >= 0.000)
                                        {
                                            //Console.WriteLine(course.Meters);
                                            if (MoreThanOne)
                                            {
                                                
                                                commands.Degree = Convert.ToInt32(angle * 100); //Collect data to send to the PLC
                                                Stopped = false;

                                                Console.Write("GPS Quality - " + nmeaBusiness.GetGPSQualityIndicator(gga.GPS_Quality_indicator));
                                                Console.Write(" // Satelites - " + gga.Number_of_SVs_in_use);
                                                Console.Write("  // Distance to " + nextPoint.Description + " - " + distance2Destination.Meters);
                                                Console.WriteLine(" // Angle - " + angle);
                                                Console.Write("    Count - " + counter);
                                                Console.WriteLine(" // Console Cycle - " + timeTotal);
                                                Console.WriteLine("  // Enable Auto: " + commands.EnableAuto);
                                                Console.Write(" OnStraight: " + commands.OnStraight.ToString());
                                                Console.WriteLine("  //  Velocity: " + commands.Velocity.ToString());
                                                Console.WriteLine("Real Velocity: " + status.VelocityTraction);
                                                
                                                //Write status data on a CLV file
                                                //if (pointModel != null)
                                                //{
                                                //    try
                                                //    {
                                                //        newLine = String.Format("{0},{1},{2},{3},{4},{5},{6}", utcDate.ToString(),
                                                //                                                    (counter).ToString(),
                                                //                                                    Convert.ToString(pointModel.Velocity),
                                                //                                                    angle,
                                                //                                                    Convert.ToString(pointModel.Description),
                                                //                                                   (nmeaBusiness.GetGPSQualityIndicator(gga.GPS_Quality_indicator)).ToString(),
                                                //                                                   (gga.Number_of_SVs_in_use).ToString());
                                                        

                                                //    }
                                                //    catch (Exception ex)
                                                //    {
                                                //        nlog.Error(ex, "DB error ");
                                                //    }
                                                //}
                                                //else
                                                //{
                                                //    newLine = String.Format("{0},{1},{2},{3},{4},{5},{6}", utcDate.ToString(),
                                                //                                                (counter).ToString(),
                                                //                                               "NOP",
                                                //                                               angle,
                                                //                                               "NOP",
                                                //                                               (nmeaBusiness.GetGPSQualityIndicator(gga.GPS_Quality_indicator)).ToString(),
                                                //                                               (gga.Number_of_SVs_in_use).ToString());
                                                    
                                                //}

                                                //csv.AppendLine(newLine);

                                                //newLine = String.Format("{0},{1},{2}", utcDate.ToString(),
                                                //                                       status.VelocityTraction.ToString(),
                                                //                                       status.ManualEnabled.ToString());

                                                //csv2.AppendLine(newLine);
                                                //Math.DivRem(counter, 500, out rest);
                                                
                                                //if (rest == 0)
                                                //{
                                                //    File.AppendAllText("C:\\ExternalAGV\\Console_CSVFile.csv", csv.ToString());
                                                //    File.AppendAllText("C:\\ExternalAGV\\PLC_CSVFile.csv", csv2.ToString());
                                                //    csv.Clear();
                                                //    csv2.Clear();
                                                //}
                                            }
                                            MoreThanOne = true;
                                        }
                                        else
                                        {
                                            Console.WriteLine(course.Meters);
                                            if (!Stopped)
                                            {
                                                Console.WriteLine("AGV stopped");
                                            }
                                            commands.Degree = 0;
                                            Stopped = true;
                                        }
                                    }

                                    if (Write2PLC)
                                    {
                                        bool retPLC = plc.WriteCommandsPLC(commands); //Send the collected data to the PLC

                                        if (!retPLC)
                                        {
                                            Console.WriteLine("Falha Escrita PLC");
                                            nlog.Write("Fault PLC writing");
                                        }
                                    }
                                    else
                                    {
                                        nlog.Write("PLC writing not enabled");
                                    }


                                    stopwatch2.Stop();
                                    nlog.Write("TIME2: " + stopwatch2.ElapsedMilliseconds);
                                    time2 = stopwatch2.ElapsedMilliseconds.ToString();
                                    stopwatch2.Reset();

                                    //stop the second timing


                                }
                                else
                                {
                                    //Start the third timing

                                    //Console.Write("\n\nENTROU T3\n\n");

                                    //collect data to send to the PLC
                                    commands.EnableAuto = false;
                                    commands.LeftLight = false;
                                    commands.RightLight = false;
                                    commands.Velocity = 2;
                                    commands.PathDeviation = 0;

                                    if (Write2PLC)
                                    {
                                        bool retPLC = plc.WriteCommandsPLC(commands); //Send the collected data to the PLC

                                        if (!retPLC)
                                        {
                                            Console.WriteLine("Falha Escrita PLC");
                                            nlog.Write("Fault PLC writing");
                                        }
                                    }
                                    else
                                    {
                                        nlog.Write("PLC writing not enabled");
                                    }

                                    if (nextPoint != null) //Verify if the route is finished
                                    {
                                        nlog.Write("Destination => Latitude:" + nextPoint.Lat + " - Longitude:" + nextPoint.Lng);
                                    }
                                    else
                                    {
                                        Console.WriteLine("\nEnd of the route\n\n");
                                        reset = true; // signals to reset the route
                                        pointCounter = 0;
                                        Console.WriteLine("Reseting...");

                                        points.ResetRoute(); //resets the route on the DB
                                        pointsString = "";
                                        //getting the reseted route
                                        pointsString = new WebClient().DownloadString("http://localhost/agvAPI/api/Points/GetAll"); 
                                        pointList = JsonConvert.DeserializeObject<List<PointsModel>>(pointsString);

                                        nextPoint = pointList[pointCounter];
                                        Console.WriteLine("Next Point: " + pointList[pointCounter].Description);

                                    }
                                    if (course == null)
                                    {
                                        nlog.Write("Course is null");
                                        Console.WriteLine("Please, move the AGV!");
                                    }
                                    //finishing the third timing

                                }


                            }

                            if (Header == "GST")
                            {
                                GSTModel gstModel = nmeaBusiness.ConvertNmea2GST(ret);
                                //Console.WriteLine(JsonConvert.SerializeObject(gstModel));
                            }

                            if (Header == "ZDA")
                            {
                                ZDAModel zdaModel = nmeaBusiness.ConvertNmea2ZDA(ret);
                                //Console.WriteLine(JsonConvert.SerializeObject(zdaModel));
                            }



                        }

                    }
                }
            }
            catch (Exception ex)
            {
                nlog.Error(ex, "GPS Reader" + ex.ToString());
            }
            finally
            {

                isBusy = false;
                counter++;
                if (counter > 5000) { counter = 1; }

                nlog.Write(" === fim2 ===");
                

                if (reset == true) //reseting the route
                {
                    var statusRestart = plc.ReadStatusPLC();
                    
                    if (statusRestart.BT_Load) //start the new route if the release button on the PLC was pressed
                    {
                        reset = false;
                        configBusiness.UpdateStartById(AGVID, true);
                        Console.WriteLine("\nStarting...\n");
                        counter = 0;
                    }
                }
                //finishing the fourth timing

            }
            nlog.Write(" === fim3 ===");
        }

    }

}//QmFndWV0RG93bg==