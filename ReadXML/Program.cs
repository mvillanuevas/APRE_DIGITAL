using System.Xml;
using System.Data.SqlClient;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading;

public class RecursiveFileSearch
{
    static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();

    static void Main()
    {
        ThreadStart childref = new ThreadStart(CallToChildThread);
        Console.WriteLine("In Main: Creating the Child thread");

        Thread childThread = new Thread(childref);
        childThread.Start();
        childThread.Join();
        // Keep the console window open in debug mode.
        Console.WriteLine("Press any key");
        Console.ReadKey();
    }

    static void WalkDirectoryTree(System.IO.DirectoryInfo root)
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        System.IO.FileInfo[] files = null;
        System.IO.DirectoryInfo[] subDirs = null;

        // First, process all the files directly under this folder
        try
        {
            files = root.GetFiles("*.*");
        }
        // This is thrown if even one of the files requires permissions greater
        // than the application provides.
        catch (UnauthorizedAccessException e)
        {
            // This code just writes out the message and continues to recurse.
            // You may decide to do something different here. For example, you
            // can try to elevate your privileges and access the file again.
            log.Add(e.Message);
        }

        catch (System.IO.DirectoryNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }

        if (files != null)
        {

            foreach (System.IO.FileInfo fi in files)
            {
                string UUID = "", SERIE = "", FOLIO = "", TOTAL = "", SUBTOTAL = "", DESCUENTO = "",
                    IVA = "", IVA_RETENIDO = "", MONEDA = "", FECHA = "", RFC_EMISOR = "", RAZON_SOCIAL_EMISOR = "",
                    RFC_RECEPTOR = "", RAZON_SOCIAL_RECEPTOR = "", TasaOCuota = "", Descripcion = "", TipoComprobante = "", TipoRelacion = "", UUIDRelacionado = "";
                // In this example, we only access the existing FileInfo object. If we
                // want to open, delete or modify the file, then
                // a try-catch block is required here to handle the case
                // where the file has been deleted since the call to TraverseTree().
                Console.WriteLine(fi.FullName);
                //XML
                XmlDocument doc = new XmlDocument();
                doc.Load(fi.FullName);

                // Add the namespace.  
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
                XmlNamespaceManager nsmgr2 = new XmlNamespaceManager(doc.NameTable);
                nsmgr2.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");

                //Fetch the specific Nodes by Attribute value.
                XmlNodeList nodeComprobante = doc.SelectNodes("//cfdi:Comprobante", nsmgr);
                //Fetch the specific Nodes by Attribute value.
                XmlNodeList nodeEmisor = doc.SelectNodes("//cfdi:Emisor", nsmgr);
                //Fetch the specific Nodes by Attribute value.
                XmlNodeList nodeReceptor = doc.SelectNodes("//cfdi:Receptor", nsmgr);
                //Fetch the specific Nodes by Attribute value.
                XmlNodeList nodeConcepto = doc.SelectNodes("//cfdi:Concepto", nsmgr);
                //Fetch the specific Nodes by Attribute value.
                XmlNodeList nodeTraslados = doc.SelectNodes("//cfdi:Traslado", nsmgr);
                //Fetch the specific Nodes by Attribute value.
                XmlNodeList nodeRetencion = doc.SelectNodes("//cfdi:Retencion", nsmgr);
                //Fetch the specific Nodes by Attribute value.
                XmlNodeList nodeRelacionados = doc.SelectNodes("//cfdi:CfdiRelacionados", nsmgr);
                //Fetch the specific Nodes by Attribute value.
                XmlNodeList nodeRelacionado = doc.SelectNodes("//cfdi:CfdiRelacionado", nsmgr);
                //Fetch the specific Nodes by Attribute value.
                XmlNodeList nodeTimbreFiscalDigital = doc.SelectNodes("//tfd:TimbreFiscalDigital", nsmgr2);

                foreach (XmlNode node in nodeComprobante)
                {
                    try
                    {
                        TipoComprobante = node.Attributes["TipoDeComprobante"].Value;
                    }
                    catch { }
                }

                
                foreach (XmlNode node in nodeRelacionados)
                {
                    try
                    {
                        TipoRelacion = node.Attributes["TipoRelacion"].Value;
                    }
                    catch { }
                    }
                foreach (XmlNode node in nodeRelacionado)
                {
                    try
                    {
                        UUIDRelacionado = node.Attributes["UUID"].Value;
                    }
                    catch { }
                }
                //Loop through the selected Nodes.
                foreach (XmlNode node in nodeComprobante)
                {
                    //Fetch the Node and Attribute values.
                    try
                    {
                        SERIE = node.Attributes["Serie"].Value;
                    }
                    catch { }
                    try
                    {
                        FOLIO = node.Attributes["Folio"].Value;
                    }
                    catch { }
                    try
                    {
                        TOTAL = node.Attributes["Total"].Value;
                    }
                    catch { }
                    try
                    {
                        SUBTOTAL = node.Attributes["SubTotal"].Value;
                    }
                    catch { }
                    try
                    {
                        MONEDA = node.Attributes["Moneda"].Value;
                    }
                    catch { }
                    try
                    {
                        FECHA = node.Attributes["Fecha"].Value;
                    }
                    catch { }

                }

                //Loop through the selected Nodes.
                foreach (XmlNode node in nodeEmisor)
                {
                    //Fetch the Node and Attribute values.
                    try
                    {
                        RFC_EMISOR = node.Attributes["Rfc"].Value;
                    }
                    catch { }
                    try
                    {
                        RAZON_SOCIAL_EMISOR = node.Attributes["Nombre"].Value;
                    }
                    catch {; }
                }

                //Loop through the selected Nodes.
                foreach (XmlNode node in nodeReceptor)
                {
                    //Fetch the Node and Attribute values.
                    try
                    {
                        RFC_RECEPTOR = node.Attributes["Rfc"].Value;
                    }
                    catch { }
                    try
                    {
                        RAZON_SOCIAL_RECEPTOR = node.Attributes["Nombre"].Value;
                    }
                    catch { }
                }

                //Loop through the selected Nodes.
                foreach (XmlNode node in nodeConcepto)
                {
                    //Fetch the Node and Attribute values.
                    try
                    {
                        DESCUENTO = node.Attributes["Descuento"].Value;
                    }
                    catch { }
                    try
                    {
                        Descripcion = node.Attributes["Descripcion"].Value;
                    }
                    catch { }
                }

                //Loop through the selected Nodes.
                foreach (XmlNode node in nodeTraslados)
                {
                    //Fetch the Node and Attribute values.
                    try
                    {
                        TasaOCuota = node.Attributes["TasaOCuota"].Value;
                        if (TasaOCuota.Contains("0.160000"))
                        {
                            IVA = node.Attributes["Importe"].Value;
                        }
                    }
                    catch { }
                }

                //Loop through the selected Nodes.
                foreach (XmlNode node in nodeTraslados)
                {
                    //Fetch the Node and Attribute values.
                    try
                    {
                        TasaOCuota = node.Attributes["TasaOCuota"].Value;
                        if (TasaOCuota.Contains("0.0600"))
                        {
                            IVA_RETENIDO = node.Attributes["Importe"].Value;
                        }
                    }
                    catch { }
                }

                //Loop through the selected Nodes.
                foreach (XmlNode node in nodeTimbreFiscalDigital)
                {
                    //Fetch the Node and Attribute values.
                    try
                    {
                        UUID = node.Attributes["UUID"].Value;
                    }
                    catch { }
                }

                string insertQuery = "insert into CFDI_Recibidos_XML (UUID,SERIE,FOLIO,TOTAL,SUBTOTAL,DESCUENTO,IVA,IVA_RETENIDO,MONEDA,FECHA,RFC_EMISOR,RAZON_SOCIAL_EMISOR,RFC_RECEPTOR,RAZON_SOCIAL_RECEPTOR,CONCEPTO,TIPO_COMPROBANTE,TipoRelacion,UUIDRelacionado) " +
                    "values ('" + UUID + "','" + SERIE + "','" + FOLIO + "',CAST('" + TOTAL.Replace(" ", "").Trim() + "' AS FLOAT),CAST('" + SUBTOTAL.Replace(" ", "").Trim() + "' AS FLOAT),CAST('" + DESCUENTO.Replace(" ", "").Trim() + "' AS FLOAT)," +
                    "CAST('" + IVA.Replace(" ", "").Trim() + "' AS FLOAT),CAST('" + IVA_RETENIDO.Replace(" ", "").Trim() + "' AS FLOAT),'" + MONEDA + "',CAST('" + FECHA + "' AS DATE),'" +
                    RFC_EMISOR + "','" + RAZON_SOCIAL_EMISOR.Replace(",", "").Replace(".", "").Replace("'", "") + "','" + RFC_RECEPTOR + "','" + RAZON_SOCIAL_RECEPTOR.Replace(",", "").Replace(".", "").Replace("'", "") + "','"
                    + Descripcion.Replace(",", "").Replace(".", "").Replace("'", "") + "','" + TipoComprobante + "','" + TipoRelacion + "','" + UUIDRelacionado + "')";

                CreateCommand(insertQuery);
                //}

                stopwatch.Stop();
                Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed.ToString("hh\\:mm\\:ss\\.fff"));
            }

            // Now find all the subdirectories under this directory.
            subDirs = root.GetDirectories();

            foreach (System.IO.DirectoryInfo dirInfo in subDirs)
            {
                // Resursive call for each subdirectory.
                WalkDirectoryTree(dirInfo);
            }
        }
    }

    static void CreateCommand(string queryString)
    {
        //SQL Connection
        string connectionString = @"Server=tcp:apre-digital.database.windows.net,1433;Initial Catalog=APRE_DB;Persist Security Info=False;User ID=apre_admin;Password=RG6wt9ea;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=100;";
        //string connectionString = @"Server=.;Database=TestAPRE;Trusted_Connection=True;";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            //Open connection
            command.Connection.Open();
            //Execute query
            command.ExecuteNonQuery();
            //Close connection
            command.Connection.Close();
        }
    }

    static void ReadBSIK()
    {
        string queryString;
        string[] lines = System.IO.File.ReadAllLines(@"C:\\Users\\he678hu\\OneDrive - EY\\PiSA\\BSIK\\BSIK_FEB_2022.txt");
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        // Display the file contents by using a foreach loop.

        foreach (string line in lines)
        {
            if (!line.Contains("MANDT") && !line.Contains("Tabla:") && !line.Contains("Campos visualiz.:") && !line.Contains("----------------------------"))
            {
                queryString = "insert into BSIK(MANDT,BUKRS,LIFNR,UMSKS,UMSKZ,AUGDT,AUGBL,ZUONR,GJAHR,BELNR,BUZEI," +
                    "BUDAT,BLDAT,CPUDT,WAERS,XBLNR,BLART,MONAT,BSCHL,ZUMSK,SHKZG,GSBER,MWSKZ,DMBTR,WRBTR,MWSTS,WMWST,BDIFF,BDIF2," +
                    "SGTXT,PROJN,AUFNR,ANLN1,ANLN2,EBELN,EBELP,SAKNR,HKONT,FKONT,FILKD,ZFBDT,ZTERM,ZBD1T,ZBD2T,ZBD3T,ZBD1P,ZBD2P," +
                    "SKFBT,SKNTO,WSKTO,ZLSCH,ZLSPR,ZBFIX,HBKID,BVTYP,REBZG,REBZJ,REBZZ,SAMNR,ZOLLT,ZOLLD,LZBKZ,LANDL,DIEKZ,MANSP," +
                    "MSCHL,MADAT,MANST,MABER,XNETB,XANET,XCPDD,XESRD,XZAHL,MWSK1,DMBT1,WRBT1,MWSK2,DMBT2,WRBT2,MWSK3,DMBT3,WRBT3," +
                    "QSSKZ,QSSHB,QBSHB,BSTAT,ANFBN,ANFBJ,ANFBU,VBUND,REBZT,STCEG,EGBLD,EGLLD,QSZNR,QSFBT) " + "values ('" + line.Remove(line.Length - 1).Replace("| |", "").Replace("|", "','") + "')";

                CreateCommand(queryString);

            }
        }
        stopwatch.Stop();
        Console.WriteLine("*** Time elapsed: {0}", stopwatch.Elapsed.ToString("hh\\:mm\\:ss\\.fff"));
    }

    static void ReadBSAK(System.IO.DirectoryInfo root)
    {
        string queryString;
        int[] caracters = { };
        System.IO.FileInfo[] files = null;
        System.IO.DirectoryInfo[] subDirs = null;
        using StreamWriter file = new("C:\\Users\\he678hu\\OneDrive - EY\\PiSA\\Log.txt", append: true);

        // First, process all the files directly under this folder
        try
        {
            files = root.GetFiles("*.*");
        }
        // This is thrown if even one of the files requires permissions greater
        // than the application provides.
        catch (UnauthorizedAccessException e)
        {
            // This code just writes out the message and continues to recurse.
            // You may decide to do something different here. For example, you
            // can try to elevate your privileges and access the file again.
            log.Add(e.Message);
        }

        catch (System.IO.DirectoryNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }

        if (files != null)
        {
            foreach (System.IO.FileInfo fi in files)
            {
                Console.WriteLine(fi.FullName);
                int cnt = 0;
                string[] lines = System.IO.File.ReadAllLines(fi.FullName, System.Text.Encoding.ASCII);

                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                // Display the file contents by using a foreach loop.

                foreach (string line in lines)
                {
                    string Cleanline = "";
                    if (!line.Contains("MANDT") && !line.Contains("Tabla:") && !line.Contains("Campos visualiz.:") && !line.Contains("----------------------------"))
                    {
                        Cleanline = line.Remove(0, 3).Remove(line.Length - 4);

                        if (caracters.Length > 0)
                        {
                            Cleanline = CleanLine(caracters, line.Remove(0, 3).Remove(line.Length - 4));
                        }

                        queryString = "insert into BSAK(MANDT,BUKRS,LIFNR,UMSKS,UMSKZ,AUGDT,AUGBL,ZUONR,GJAHR,BELNR,BUZEI,BUDAT,BLDAT,CPUDT," +
                            "WAERS,XBLNR,BLART,MONAT,BSCHL,ZUMSK,SHKZG,MWSKZ,DMBTR,WRBTR,MWSTS,WMWST,BDIFF,SGTXT,SAKNR,ZFBDT,ZTERM,ZBD1T," +
                            "ZBD2T,ZBD3T,ZBD1P,ZBD2P,SKFBT,SKNTO,WSKTO,ZLSCH,ZLSPR,ZBFIX,REBZG,REBZJ) " + "values ('" + Cleanline.Replace(",", "").Replace("'", "").Replace("|", "','") + "')";

                        //try
                        //{
                        CreateCommand(queryString);
                        //}
                        //catch(Exception ex)
                        /*{
                            file.WriteLine(Cleanline.Replace("'", "").Replace("'", ""));
                            cnt = cnt + 1;
                        }*/

                    }
                    else
                    {
                        if (line.Contains("MANDT"))
                        {
                            caracters = CountCaracters(line.Remove(line.Length - 1).Replace("| |", ""));
                        }
                    }
                }
                stopwatch.Stop();
                Console.WriteLine(cnt);
                Console.WriteLine("*** Time elapsed: {0}", stopwatch.Elapsed.ToString("hh\\:mm\\:ss\\.fff"));
            }
            // Now find all the subdirectories under this directory.
            subDirs = root.GetDirectories();

            foreach (System.IO.DirectoryInfo dirInfo in subDirs)
            {
                // Resursive call for each subdirectory.
                ReadBSAK(dirInfo);
            }
        }
    }

    static int[] CountCaracters(string line)
    {
        int[] caracters = { };
        if (line.Contains("MANDT"))
        {
            string[] columns = line.Split('|');

            foreach (string column in columns)
            {
                caracters = caracters.Append(column.Length).ToArray();
            }
        }

        return caracters;
    }

    static string CleanLine(int[] caracters, string line)
    {
        int index = 0;
        string cleanLine = "";

        foreach (int caracter in caracters)
        {
            string tmp = line.Substring(index, caracter).Replace("|", " ").Trim();
            cleanLine = cleanLine + "|" + tmp;
            index = caracter + index + 1;
        }
        return cleanLine.Remove(0, 1);
    }

    static void Vendormaster()
    {
        string queryString;
        using StreamWriter file = new("C:\\Users\\he678hu\\OneDrive - EY\\PiSA\\Log.txt", append: true);
        string[] lines = System.IO.File.ReadAllLines(@"C:\\Users\\he678hu\\OneDrive - EY\\PiSA\\LFA1_PiSA.txt", System.Text.Encoding.ASCII);
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        // Display the file contents by using a foreach loop.

        foreach (string line in lines)
        {
            if (!line.Contains("MANDT"))
            {
                queryString = "insert into VENDOR_MASTER(Id,MANDT,LIFNR,LAND1,NAME1,NAME2,NAME3,NAME4,ORT01,ORT02,PFACH," +
                    "PSTLZ,REGIO,STRAS,ADRNR,ANRED,BEGRU,BRSCH,ERDAT,ERNAM,LOEVM,SPERR,SPERM,STCD1,TELF1,TELF2," +
                    "VBUND,STCEG,SPERZ) " + "values ('" + line.Replace("'", "").Replace("|", "','") + "')";

                try
                {
                    CreateCommand(queryString);
                }
                catch (Exception ex)
                {
                    file.WriteLine(line);
                }

            }
        }
        stopwatch.Stop();
        Console.WriteLine("*** Time elapsed: {0}", stopwatch.Elapsed.ToString("hh\\:mm\\:ss\\.fff"));
    }

    static string GetBlob(string containerName, string fileName)
    {
        string connectionString = $"DefaultEndpointsProtocol=https;AccountName=apredigital;" +
            $"AccountKey=UamjIsnTRuZ2B5xmoPSPFTqzIQsIicHLBTaSAY2ol58mtNvisGewGcYBArPkjy0BfN79C0SX6X5g+AStuMXbHw==;" +
            $"EndpointSuffix=core.windows.net";

        // Setup the connection to the storage account
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
        Console.WriteLine("Connect to the blob storage");
        // Connect to the blob storage
        CloudBlobClient serviceClient = storageAccount.CreateCloudBlobClient();
        // Connect to the blob container
        Console.WriteLine("Connect to the blob container");
        CloudBlobContainer container = serviceClient.GetContainerReference($"{containerName}");
        // Connect to the blob file
        CloudBlockBlob blob = container.GetBlockBlobReference($"{fileName}");
        // Get the blob file as text
        Console.WriteLine("Get the blob file as text");
        string contents = blob.DownloadTextAsync().Result;
        Console.WriteLine("Return text");
        return contents;
    }

    public static void CallToChildThread()
    {
        //Vendormaster();
        //ReadBSIK();


        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();


        // Start with drives if you have to search the entire computer.
        string[] drives = { "C:\\Users\\he678hu\\OneDrive - EY\\PiSA\\Cab Logistic\\ARCHIVOS XML\\RECIBIDOS\\2018" };
        //string[] drives = {"C:\\Users\\he678hu\\OneDrive - EY\\PiSA\\BSAK"};

        foreach (string dr in drives)
        {
            System.IO.DriveInfo di = new System.IO.DriveInfo(dr);
            stopwatch.Start();
            // Here we skip the drive if it is not ready to be read. This
            // is not necessarily the appropriate action in all scenarios.
            if (!di.IsReady)
            {
                Console.WriteLine("*** The drive {0} could not be read", di.Name);
                continue;
            }
            System.IO.DirectoryInfo rootDir = new DirectoryInfo(dr);
            WalkDirectoryTree(rootDir);
            //ReadBSAK(rootDir);
            stopwatch.Stop();
            Console.WriteLine("### Time elapsed: {0}", stopwatch.Elapsed.ToString("hh\\:mm\\:ss\\.fff"));
        }

        // Write out all the files that could not be processed.
        Console.WriteLine("Files with restricted access:");
        foreach (string s in log)
        {
            Console.WriteLine(s);
        }
    }
}