using EasyHook;
using InputManager;
using NAudio.Wave;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
//using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;


namespace DvFans19
{
    public partial class Form1 : Form
    {

        /* La varaible mas importantes */
        Label developerlabel = null;
        Boolean Empezado = false;
        Boolean DBcargada = false;
        Boolean reproduciendo = false;
        int index_Equipo_L = -1;
        int Equipo_L = -1;
        int Goles_L = -1;
        int index_Equipo_V = -1;
        int Equipo_V = -1;
        int Goles_V = -1;
        int Tiempo_Juego = -1;
        Boolean himnos_reproducidos = false;
        DateTime Ultima_reproducion;
        int EquipoUltimaReproduccion = -1;
        List<string> id_equipos = new List<string>();
        List<string> equipos = new List<string>();
        List<string> abreviaturasEquipos = new List<string>();
        List<string> exc_id_equipos = new List<string>();
        List<string> exc_words = new List<string>();
        List<String> canciones_Local = new List<string>();
        List<String> canciones_Local_Respaldo = new List<string>();
        List<String> canciones_Visitante = new List<string>();
        List<String> canciones_Visitante_Respaldo = new List<string>();
        List<String> cancionespr_Local = new List<string>();
        List<String> cancionespr_Local_Respaldo = new List<string>();
        List<String> cancionespr_Visitante = new List<string>();
        List<String> cancionespr_Visitante_Respaldo = new List<string>();
        List<String> canciones_neutrales = new List<string>();
        List<String> canciones_neutrales_respaldo = new List<string>();
        int intentos_de_captura_fallidos = 0;
        int intentos_antes_de_cancelar = 100;


        int Linferior_CL = -1;
        int Linferior_CV = -1;
        int Linferior_CN = -1;
        Notyform notyform = new Notyform();
        WaveOutEvent outputDevice;
        int LastTeamsong = -1;
        /* Inicia en false se vuelve true cuando despues del escaneo si es falsa y se detectan equipos con
        con canticos guardados
        Se vuelve false cuando no hay canticos de los equipos encontrados y cuando cambian los equipos
        en el marcador mini
        cuando se cierra el juego
        cuando no se detecte marcador ni franja durante 5 capturas
        es decir cuando el partido acaba y no se reinicia. */



        List<string> ids_equipos_detectados = new List<string>();
        const UInt32 WM_KEYDOWN = 0x0100;
        const int VK_F5 = 0x74;
        const int VK_RETURN = 0x0D; //Enter
        const int VK_ESCAPE = 0x1B; //ESC
        Boolean calibrando = false;
        System.Diagnostics.Process proc;
        Form f;
        Panel reci;
        private static Label myLabel;

        //Variables DEl hud resueltas
        int H_t = 0;
        int H_l = 0;
        int H_r = 0;
        int H_b = 0;

        public Form1()
        {

            InitializeComponent();
            //Empezar
            RegisterHotKey(this.Handle, 1, 2, (int)Keys.F2);
            //Terminar
            RegisterHotKey(this.Handle, 12, 2, (int)Keys.F4);
            //Calibrar
            RegisterHotKey(this.Handle, 11, 2, (int)Keys.F3);
            PreStart();
            CargarDBEquipos();
            //  myLabel = label5;


            // Application.EnableVisualStyles();
            // Application.Run(f);


           
            Cargar_Canciones_Neutrales();
        }
        #region Mostra Datos para desarrolladores
        public void MostrarLog()
        {
            if (developerlabel == null)
            {
                Form DeveloperLog = new Form();
                DeveloperLog.ShowInTaskbar = false;
                DeveloperLog.StartPosition = FormStartPosition.Manual;
                DeveloperLog.Location = new Point(2, 2);
                //DeveloperLog.Left = 5;
                DeveloperLog.BringToFront();
                DeveloperLog.Width = 1000;
                DeveloperLog.Height = 10;
                DeveloperLog.FormBorderStyle = FormBorderStyle.None;
                DeveloperLog.BackColor = System.Drawing.Color.Green;
                DeveloperLog.TransparencyKey = System.Drawing.Color.Green;
                developerlabel = new Label();
                developerlabel.Paint += new System.Windows.Forms.PaintEventHandler(TestForm_Paint);
                developerlabel.Font = new Font("Arial", 13, FontStyle.Bold);
                developerlabel.BackColor = Color.Transparent;
                developerlabel.ForeColor = System.Drawing.Color.Red;
                developerlabel.AutoSize = true;
                DeveloperLog.TopMost = true;
                DeveloperLog.Controls.Add(developerlabel);
                DeveloperLog.Show();
                developerlabel.Text = "LOG";
            }
        }
        public Boolean DevelopersLog()
        {
            if(developerlabel == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public void WriteLog(String text)
        {
            if (developerlabel != null)
            {
                developerlabel.Text = text;
            }
        }
        private void TestForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
           // e.Graphics.DrawString("Header", this.Font, SystemBrushes.WindowText, new Point(1, 1));
        }
        #endregion
        String s_subprocceso_o_var;
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            #region funciones de mover el form de calibracion de hud
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == 2)
            {
                //Mover Arriba
                f.Top -= 5;
            }
            else if (m.Msg == 0x0312 && m.WParam.ToInt32() == 3)
            {
                //Mover Abajo
                f.Top += 5;
            }
            else if(m.Msg == 0x0312 && m.WParam.ToInt32() == 4)
            {
                //Mover Izquierda
                f.Left -= 5;
            }
            else if(m.Msg == 0x0312 && m.WParam.ToInt32() == 5)
            {
                //Mover Derecha
                f.Left += 5;
            }
            else if(m.Msg == 0x0312 && m.WParam.ToInt32() == 6)
            {
                //Hacer mas Ancho
                f.Width += 10;
                reci.Width += 10;
               //f.Left -= 5;

            }
            else if(m.Msg == 0x0312 && m.WParam.ToInt32() == 7)
            {
                //Hacer Mas angosto
                f.Width -= 10;
                reci.Width -= 10;
                //   f.Left += 5;
            }
            else if (m.Msg == 0x0312 && m.WParam.ToInt32() == 8)
            {
                //Hacer mas Alto
                f.Height += 10;
                reci.Height += 10;
                
               // f.Top -= 5;
            }
            else if (m.Msg == 0x0312 && m.WParam.ToInt32() == 9)
            {
                //Hacer Mas Bajo
                f.Height -= 10;
                reci.Height -= 10;
                // f.Top += 5;
            }
            #endregion
            else if (m.Msg == 0x0312 && m.WParam.ToInt32() == 11)
            {

            #region CONTROL + F3

               // var prob = System.Diagnostics.Process.GetProcessesByName("FIFA19");
                var prob = System.Diagnostics.Process.GetProcessesByName("taskmgr");
                if (prob.Length > 0)
                {
                    proc = prob[0];
                    if (calibrando == false)
                    {
                        A_Configurar_HUD();
                    }
                    else
                    {
                        Terminar_configuracionHud();
                        f.Close();
                        f = null;
                        calibrando = false;
                    }

                }
                #endregion
            }
            else if (m.Msg == 0x0312 && m.WParam.ToInt32() == 1)
            {
                #region CONTROL + F2
                
               // var prob = System.Diagnostics.Process.GetProcessesByName("FIFA19");
                var prob = System.Diagnostics.Process.GetProcessesByName("taskmgr");

                if (prob.Length > 0)
                {
                    proc = prob[0];
                    proc.EnableRaisingEvents = true;
                    proc.Exited += (sender, e) => {
                        // MessageBox.Show("a");
                        CancelarTodo();
                        notyform = new Notyform();
                        notyform.StartPosition = FormStartPosition.Manual;
                        notyform.Top = 100;
                        notyform.Left = 100;
                        notyform.Controls[1].Text = "FIFA se ha cerrado";
                        notyform.Controls[0].Text = "";
                        notyform.TopMost = true;
                        Escondernotificacion();
                        notyform.ShowDialog();
                        

                    };
                    //Comprobamos que el tamaño de la ventana sea el mismo del
                    // .ini si ha cambiado solicitar que se vuelva a recalibrar

                    if (Comprobar_escala_hud())
                    {
                        if (Empezado == false)
                        {

                            //Esta Lista contiene los strings en el siguiente orden
                            /*
                             * [0] Tiempo JUEGO
                             * [1] MARCADOR_FRANJA
                             * [2] IMGEQUIPOLOCAL
                             * [3] EQUIPO VISITANTE
                             * [4] NOMBRE DE EQUIPOs QUE APARECE EN CARGA INTERACTIVA O MARCADOR
                             * 
                             */

                            List<String> captura_obtenida = Obtener_captura(proc);
                            if (captura_obtenida.Count > 0)
                            {
                                String superstring = "";
                                foreach (string i in captura_obtenida)
                                {
                                    superstring += "(" + i + ")";
                                }

                                //Leemos los equipos de la carga interactiva y la franja



                                //Primero leemos los de la franja
                                (int tl, int inxtl) = LeerEquipo(captura_obtenida[2]);
                                (int tv, int inxtv) = LeerEquipo(captura_obtenida[3]);
                                superstring += "(" + tl + ")";
                                superstring += "(" + tv + ")";

                                if ((tl == -1) && (tv == -1))
                                {
                                    //No encontramos ningun equipo asi que no iniciamos el tick
                                    //Leemos ahora en la carga interactiva que seria captura_obtenida[4]


                                    //Hay que hacer la funcion de analizar texto equipos en la ci
                                    List<(int a, int b)> ttt = LeerEquipoCI(captura_obtenida[4]);


                                    if (ttt.Count > 0)
                                    {
                                        //Hay equipos
                                        //como son de la carga interactiva los goles serian 0 y el tiempo -1 ya que 
                                        //el tiempo solo lo actualiza la franja y el marcador pero podemos optar por
                                        if (ttt.Count > 0)
                                        {
                                            //Hay local
                                            Equipo_L = ttt[0].a;
                                            index_Equipo_L = ttt[0].b;
                                            Cargar_Canciones(Equipo_L.ToString(), 1);
                                            Goles_L = 0;
                                        }
                                        if (ttt.Count > 1)
                                        {
                                            //Hay visitante
                                            index_Equipo_V = ttt[0].b;
                                            Equipo_V = ttt[1].a;
                                            Cargar_Canciones(Equipo_V.ToString(), 2);
                                            Goles_V = 0;
                                        }
                                        //Comprobamos que almenos alla cancioes del equipo
                                        // Si los equipos tienen canciones en la carpeta empezamos el tick si no, cancelamos todo
                                        // y reiniciamos las variables
                                        // reproducir himnos si los equipos tienen
                                        if (DevelopersLog())
                                        {
                                            // "JUN-BUC 0-0 [90'] 9:37AM, *"
                                            WriteLog("Cantidad Audios Local: ["+ canciones_Local.Count.ToString()+","+cancionespr_Local.Count.ToString() + "] - ["+ canciones_Visitante.Count.ToString()+", "+cancionespr_Visitante.Count.ToString() + "] - N ["+canciones_neutrales.Count.ToString()+"]");
                                            //MessageBox.Show("Cantidad Audios Local: [" + canciones_Local.Count.ToString() + "," + cancionespr_Local.Count.ToString() + "] - [" + canciones_Visitante.Count.ToString() + ", " + cancionespr_Visitante.Count.ToString() + "] - N [" + canciones_neutrales.Count.ToString() + "]");

                                        }
                                        if ((canciones_Local.Count > 0) || (canciones_Visitante.Count > 0))
                                        {
                                           
                                            ReproducirHimnos();
                                            //Insertar Aqui Funcion Crear Playlist
                                            Empezado = true;
                                            // empezamos a el tick
                                            Hover.Start();
                                        }
                                        else
                                        {
                                            CancelarTodo();
                                        }
                                    }

                                    Tiempo_Juego = -1;
                                }
                                else
                                {

                                    //Encontramos almenos un equipo
                                    //Sacamos el split de guion para obtener los goles
                                    String[] marcador = captura_obtenida[1].Split('-');
                                    Equipo_L = tl;
                                    Equipo_V = tv;
                                    if (tl != -1)
                                    {
                                        // Equipo_L = tl;
                                        index_Equipo_L = inxtl;
                                        Cargar_Canciones(Equipo_L.ToString(), 1);
                                        if (marcador.Length > 1)
                                        {
                                            int i;
                                            if (int.TryParse(marcador[0], out i))
                                            {
                                                Goles_L = i;
                                            }
                                        }
                                        superstring += "(" + canciones_Local.Count + ")";
                                        superstring += "(" + Goles_L + ")";
                                    }
                                    if (tv != -1)
                                    {
                                        index_Equipo_V = inxtv;
                                        //  Equipo_V = tv;
                                        Cargar_Canciones(Equipo_V.ToString(), 2);
                                        if (marcador.Length > 1)
                                        {
                                            int i;
                                            if (int.TryParse(marcador[1], out i))
                                            {
                                                Goles_V = i;
                                            }
                                        }
                                        superstring += "(" + canciones_Visitante.Count + ")";
                                        superstring += "(" + Goles_V + ")";
                                    }
                                    //Como iniciamos desde la franja debemos leer el tiempo de
                                    //juego y convertirlo a int solo minutos los segundos no nos interesan
                                    //Tiempo_Juego = -1;
                                    Tiempo_Juego = -1;
                                    int iii;
                                    if (int.TryParse(captura_obtenida[0].Split(':')[0], out iii))
                                    {
                                        Tiempo_Juego = iii;
                                    }


                                    // Si los equipos tienen canciones en la carpeta empezamos el tick si no, cancelamos todo
                                    // y reiniciamos las variables
                                    WriteLog(abreviaturasEquipos[index_Equipo_L] + "-" + abreviaturasEquipos[index_Equipo_V] + " " + Goles_L + "-" + Goles_V + " [" + TiempoJUEGO + "'] " + DateTime.Now.ToString());

    
                                    // "JUN-BUC 0-0 [90'] 9:37AM, *"
                                    WriteLog("Cantidad Audios Local: [" + canciones_Local.Count.ToString()+","+cancionespr_Local.Count.ToString() + "] - ["+ canciones_Visitante.Count.ToString()+", "+cancionespr_Visitante.Count.ToString() + "] - N ["+canciones_neutrales.Count.ToString()+"]");
                                    // MessageBox.Show("Cantidad Audios Local: [" + canciones_Local.Count.ToString() + "," + cancionespr_Local.Count.ToString() + "] - [" + canciones_Visitante.Count.ToString() + ", " + cancionespr_Visitante.Count.ToString() + "] - N [" + canciones_neutrales.Count.ToString() + "]");

                                
                                    if ((canciones_Local.Count > 0) || (canciones_Visitante.Count > 0))
                                    {
                                        Empezado = true;
                                        // empezamos  el tick
                                        Hover.Start();

                                        if (Tiempo_Juego == 0)
                                        {
                                            ReproducirHimnos();
                                        }
                                    }
                                    else
                                    {
                                        CancelarTodo();
                                    }

                                   // MessageBox.Show(superstring);
                                }

                            }
                            //Forzar sonar cancion con F2
                            //Puede convertirse en una funcion como (Pedir aliento a los hinchas)
                            // Si logramos leer los gamepad

                            else if (Empezado == true)
                            {
                                // Reproducir_Canciones();
                            }
                        }
                    }
                    else
                    {
                        // Solicitar Recalibrar con Control F3 (Enviando notificacion)
                        notyform = new Notyform();
                        notyform.Controls[1].Text = "Por favor calibre el Hud";
                        notyform.Controls[0].Text = "";
                        Escondernotificacion();
                        notyform.ShowDialog();
                    }
                    
                }
                #endregion
            }
            else if (m.Msg == 0x0312 && m.WParam.ToInt32() == 12)
            {
                
            #region Terminar CONTROL + F4
                var prob = System.Diagnostics.Process.GetProcessesByName("FIFA19");
                // var prob = System.Diagnostics.Process.GetProcessesByName("taskmgr");

                if (prob.Length > 0)
                {
                    
                        if (Empezado == true)
                        {
                        Empezado = false;
                        Hover.Stop();
                        }
                }
            }
            #endregion
            base.WndProc(ref m);
        }
        #region Cancelar todo
        public void CancelarTodo()
        {
            Empezado = false;
            DBcargada = false;
            Equipo_L = -1;
            Goles_L = -1;
            Equipo_V = -1;
            Goles_V = -1;
            Tiempo_Juego = -1;
           // DateTime Ultima_reproducion ;
            EquipoUltimaReproduccion = -1;
            id_equipos = new List<string>();
            equipos = new List<string>();
            exc_id_equipos = new List<string>();
            exc_words = new List<string>();
            canciones_Local = new List<string>();
            canciones_Visitante = new List<string>();
            Hover.Stop();
            WriteLog("Cancelado");
            if (f.Visible) { f.Close(); }
        }
        #endregion

        #region Calibrar hud
        private Boolean Comprobar_escala_hud()
        {
            //Comprobamos que la resolucion de la pantalla sea el mismo del ini
            //comprobamos que el estado sea el mismo (Con bordes o pantalla completa)
            //Que la posicion sea el mimo
            //si todo coincide devolvemos true
            //si no coincide devolvemos false
            if(File.Exists("hudcords.ini"))
            {
                //Leemos
                string[] lines = System.IO.File.ReadAllLines("hudcords.ini");
                /*EJEMPLO
                 * ARRIBA T=x
                 * ABAJO B=x
                 * IZQUIERDA L=x
                 * DERECHA R=X
                 * */
                foreach (String linea in lines)
                {
                    var pre = linea.Split('=');
                    if(pre[0] == "T")
                    {
                        H_t = Int32.Parse(pre[1]);
                    }else if(pre[0] == "B")
                    {
                        H_b = Int32.Parse(pre[1]);
                    }
                    else if (pre[0] == "L")
                    {
                        H_l = Int32.Parse(pre[1]);
                    }
                    else if (pre[0] == "R")
                    {
                        H_r = Int32.Parse(pre[1]);
                    }
                    else if (pre[0] == "DEVLOG")
                    {
                        int pDevelopersLog = Int32.Parse(pre[1]);
                        if (pDevelopersLog == 1)
                        {
                            MostrarLog();
                        }
                    }
                }

                Calibrar_secciones();
            }
            else
            {
                //Notificar Configurar HUD
                notyform = new Notyform();
                notyform.Controls[1].Text = "Por favor calibre el Hud usando Ctrl + F3";
                notyform.Controls[0].Text = "";
                Escondernotificacion();
                notyform.ShowDialog();
                
                // MessageBox.Show("Porfavor configure el Hud Ctrl + F3");
                //podemos iniciar inmediatamente la configuracion
                A_Configurar_HUD();


            }
            return true;
        }
       
        private void A_Configurar_HUD()
        {
            calibrando = true;
            RegisterHotKey(this.Handle, 2, 2, (int)Keys.Up);
            RegisterHotKey(this.Handle, 3, 2, (int)Keys.Down);
            RegisterHotKey(this.Handle, 4, 2, (int)Keys.Left);
            RegisterHotKey(this.Handle, 5, 2, (int)Keys.Right);
            RegisterHotKey(this.Handle, 6, 2, (int)Keys.W);
            RegisterHotKey(this.Handle, 7, 2, (int)Keys.Q);
            RegisterHotKey(this.Handle, 8, 2, (int)Keys.S);
            RegisterHotKey(this.Handle, 9, 2, (int)Keys.A);
            f = new Form();
            f.FormBorderStyle = FormBorderStyle.None;
            f.TransparencyKey = System.Drawing.Color.Green;
            f.StartPosition = FormStartPosition.Manual;
            f.Top = 100;
            f.Left = 100;
            f.BackColor = Color.Purple;
            reci = new Panel();
            reci.BackColor = System.Drawing.Color.Green;
            reci.Left = 5;
            reci.Top = 5;
            f.Width = 700;
            f.Height = 395;
            reci.Width = 690;
            reci.Height = 385;
            f.Controls.Add(reci);
            f.TopMost = true;
            
            notyform = new Notyform();
            notyform.StartPosition = FormStartPosition.Manual;
            notyform.Top = 100;
            notyform.Left = 100;
            notyform.TopMost = true;
            notyform.Show(this);
            f.ShowDialog(this);
            


        }
 
        private void Terminar_configuracionHud()
        {
           
            UnregisterHotKey(this.Handle, 2);
            UnregisterHotKey(this.Handle, 3);
            UnregisterHotKey(this.Handle, 4);
            UnregisterHotKey(this.Handle, 5);
            UnregisterHotKey(this.Handle, 6);
            UnregisterHotKey(this.Handle, 7);
            UnregisterHotKey(this.Handle, 8);
            UnregisterHotKey(this.Handle, 9);
            //Obtenr Cordenadas y tamaño de la aplicacion y su estado de ventana
            //   MessageBox.Show("Top: " + f.Top.ToString() + " Left: " + f.Left.ToString());
            //Guardar El .ini de esta configuración
            /*EJEMPLO
                 * ARRIBA T=x
                 * ABAJO B=x
                 * IZQUIERDA L=x
                 * DERECHA R=X
                 * */
            Comprobar_escala_hud();
            if (File.Exists("hudcords.ini"))
            {
                System.IO.File.WriteAllText("hudcords.ini", string.Empty);
            }
            else { 
               var mf = File.Create("hudcords.ini");
                mf.Close();
            }
            H_t = f.Top + 5;
            H_b = f.Bottom - 5;
            H_l = f.Left + 5;
            H_r = f.Right - 5;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("hudcords.ini"))
            {
               
                file.WriteLine("T="+ (f.Top + 5).ToString());
                file.WriteLine("B="+ (f.Bottom - 5).ToString());
                file.WriteLine("L="+ (f.Left + 5).ToString());
                file.WriteLine("R="+ (f.Right - 5).ToString());
                if (DevelopersLog())
                {
                    file.WriteLine("DEVLOG=1");
                }
                else
                {
                    file.WriteLine("DEVLOG=0");
                }
                
                file.Close();
            }
            Calibrar_secciones();
            //Notificar que esta listo para jugar 
            if (notyform.Visible)
            {
               
                notyform.Controls[1].Text = "Ya puedes seguir jugando!";
                notyform.Controls[0].Text = "";
                Escondernotificacion();
            }
            
            // notyform.Close();
            // MessageBox.Show("Ya puedes seguir jugando!");
        }
        public void Escondernotificacion()
        {
          
                Task.Delay(4000).ContinueWith(t => {
                    Invoke(new MethodInvoker(() =>
                    {
                        notyform.Close();
                    }));
                });
        
        }
        #endregion

        #region Analizador de textos para saber que equipo es
        public void CargarDBEquipos()
        {
            //Cargar equipos desde DB
            try
            {
                string[] lines = System.IO.File.ReadAllLines(@"DBEquipos\equipos.txt");
                foreach (string line in lines)
                {
                    String[] sp_a = line.Split('-');

                    String[] palabras_name = sp_a[1].ToLower().Split(' ');
                    foreach (string part in palabras_name)
                    {
                        id_equipos.Add(sp_a[0]);
                        equipos.Add(part);
                        abreviaturasEquipos.Add(sp_a[2]);
                    }

                }
                string[] exc_lines = System.IO.File.ReadAllLines(@"DBEquipos\excepciones.txt");

                foreach (string line in exc_lines)
                {
                    String[] sp_a = line.Split('-');
                    exc_id_equipos.Add(sp_a[0]);
                    exc_words.Add(sp_a[1].ToLower());
                }
                DBcargada = true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error al cargar base de dato de los equipos");
            }
        }
        public (int,int) LeerEquipo(String TextoCE)
        {
            /*
             * Este es un algorimo sencillo podria mejorarse usando por ejemplo silabas unicas etc
             */
            int aretornar = -1;
            int aretornarb = -1;
            //Analizar Equipos (MEJORADO)
            List<int> linea_equipos_detectados = new List<int>();
            List<string> equipos_detectados = new List<string>();
            List<int> index_equipos_detectados = new List<int>();

            string chech = TextoCE.Replace(" ", "").ToLower();
            // MessageBox.Show(chech);
            //Recorremos la tabla equipos en el texto recivido
            //Si encontramos alguna coincidencia entre un equipo y el texto recivido
            // Buscamos sus excepcion de palabras y si encontramos la eliminamos del texto recivido
            for (int w = 0; w <= equipos.Count - 1; w++)
            {
               
                if (chech.IndexOf(equipos[w]) != -1)
                {
                    
                    if (exc_id_equipos.IndexOf(id_equipos[w]) != -1)
                    {
                        String[] wtd = exc_words[exc_id_equipos.IndexOf(id_equipos[w])].Split(',');
                        for (int y = 0; y <= wtd.Length - 1; y++)
                        {
                            TextoCE = TextoCE.Replace(" ", "").ToLower().Replace(wtd[y], "");
                        }
                    }
                }
            }
            
            //Recorremos nuevamente el texto recivido  para encontrar un equipo
            for (int w = 0; w <= equipos.Count - 1; w++)
            {
                if ((chech.IndexOf(equipos[w]) != -1) && (!equipos_detectados.Contains(equipos[w])))
                {
                    //  index_equipos_detectados.Add(chech.IndexOf(equipos[w]));
                    // ids_equipos_detectados.Add(id_equipos[w]);  
                    //  equipos_detectados.Add(equipos[w]);


                    //Encontramos un equipo lo retornamos y damos por terminado el ciclo for
                    aretornar = Int32.Parse(id_equipos[w]);
                    aretornarb = w;
                    w = equipos.Count;
                }
            }
            return (aretornar, aretornarb);
        }

        public List<(int,int)> LeerEquipoCI(String TextoCE)
        {
            /*
             * Este es un algorimo sencillo podria mejorarse usando por ejemplo silabas unicas etc
             */
            List<(int,int)> aretornar = new List<(int,int)>();
            //Analizar Equipos (MEJORADO)
            List<int> linea_equipos_detectados = new List<int>();
            List<string> equipos_detectados = new List<string>();
            List<int> index_equipos_detectados = new List<int>();

            string chech = TextoCE.Replace(" ", "").ToLower();
            // MessageBox.Show(chech);
            //Recorremos la tabla equipos en el texto recivido
            //Si encontramos alguna coincidencia entre un equipo y el texto recivido
            // Buscamos sus excepcion de palabras y si encontramos la eliminamos del texto recivido
            for (int w = 0; w <= equipos.Count - 1; w++)
            {

                if (chech.IndexOf(equipos[w]) != -1)
                {

                    if (exc_id_equipos.IndexOf(id_equipos[w]) != -1)
                    {
                        String[] wtd = exc_words[exc_id_equipos.IndexOf(id_equipos[w])].Split(',');
                        for (int y = 0; y <= wtd.Length - 1; y++)
                        {
                            TextoCE = TextoCE.Replace(" ", "").ToLower().Replace(wtd[y], "");
                        }
                    }
                }
            }

            //Recorremos nuevamente el texto recivido  para encontrar un equipo
            for (int w = 0; w <= equipos.Count - 1; w++)
            {
                if ((chech.IndexOf(equipos[w]) != -1) && (!equipos_detectados.Contains(equipos[w])))
                {
                    //  index_equipos_detectados.Add(chech.IndexOf(equipos[w]));
                    // ids_equipos_detectados.Add(id_equipos[w]);  
                    //  equipos_detectados.Add(equipos[w]);


                    //Encontramos un equipo lo retornamos
                    aretornar.Add((Int32.Parse(id_equipos[w]),w));
                    //Si ya tenemos los dos equipos damos por terminado el ciclo for
                    if (aretornar.Count == 2)
                    {
                        w = equipos.Count;
                    }
                    
                }
            }
            //Quien es local o visitante no importa por ahora
            return aretornar;
        }
        #endregion

        #region Servicio de Captura (todavia no terminado)
        private void Hover_Tick(object sender, EventArgs e)
        {
            //Si estamos reproduciendo algo ignoramos todo esto
            if(intentos_de_captura_fallidos == intentos_antes_de_cancelar)
            {
                CancelarTodo();
            }
            if (reproduciendo == false)
            {
                int suma = canciones_Local.Count + cancionespr_Local.Count + canciones_Visitante.Count + cancionespr_Visitante.Count + canciones_neutrales.Count;
                WriteLog("Corriendo Servicio Audios Disponibles = " + suma.ToString());
                
                //Si hay canciones disponible procedemos
                if (suma > 0)
                {

                    IntPtr hWnd = proc.MainWindowHandle;
                    var rect = new User32.Rect();
                    User32.GetWindowRect(proc.MainWindowHandle, ref rect);
                    if (rect.left == -32000)
                    {
                        // El juego esta minimizado no hacemos nada
                        //Hay que hacer un codigo que cuente los reintentos y uno que detecte cuando se cierre
                        //El fifa para cancelarlo todo
                        WriteLog("Juego Minimizado");
                        intentos_de_captura_fallidos++;
                    }
                    else
                    {
                        Boolean reproducir_siguiente_one = false;
                        //Capturamos el marcador y la franja from start y le aplicamos OCR
                        //Esta Lista contiene los strings en el siguiente orden
                        /*
                         * [0] Tiempo JUEGO
                         * [1] MARCADOR_FRANJA
                         * [2] IMGEQUIPOLOCAL
                         * [3] EQUIPO VISITANTE
                         * [4] NOMBRE DE EQUIPOs QUE APARECE EN CARGA INTERACTIVA O MARCADOR
                         * 
                         */
                        List<String> captura_obtenida = Obtener_captura(proc);
                        
                        //  MessageBox.Show(superstringb);
                        //Si no hay equipos en la franja entonces usamos los datos del marcador
                        //Debemos crear la funcion Analizar equipos

                        //Nos aseguramos de haber recibido una captura antes de seguir
                        if (captura_obtenida.Count > 0)
                        {
                            String superstringb = "";
                            foreach (string i in captura_obtenida)
                            {
                                superstringb += "(" + i + ")";
                            }
                            //Primero leemos los de la franja
                            (int tl, int inxtl) = LeerEquipo(captura_obtenida[2]);
                            (int tv, int inxtv) = LeerEquipo(captura_obtenida[3]);
                            // MessageBox.Show(tl+","+tv);
                            WriteLog(superstringb);
                            if ((tl == -1) && (tv == -1))
                            {
                                //No encontramos ningun equipo en la franja
                                //Leemos ahora en el marcador que seria captura_obtenida[4]
                                //Tengo que hacer la funcion de analizar texto equipos en la ci
                                List<int> CapturaMarcador = new List<int>();
                                //MessageBox.Show(captura_obtenida[4]);
                                String[] preCapturaMarcador = captura_obtenida[4].Split(',');
                                if (preCapturaMarcador.Length > 1)
                                {
                                    foreach (String a in preCapturaMarcador)
                                    {
                                        int i = -1;
                                        if (int.TryParse(a, out i))
                                        {
                                            CapturaMarcador.Add(i);
                                        }
                                        else
                                        {
                                            CapturaMarcador.Add(i);
                                        }

                                    }
                                    /*
                                     * Debe devolver algo como: yyy
                                     * [0] Id local
                                     * [1] Id Visitante
                                     * [2] Goles Local
                                     * [3] Goles Visitante
                                     * [4] Minutos
                                     * [5] Index Local
                                     * [6] Index Visitante
                                     */
                                    if (CapturaMarcador.Count > 0)
                                    {
                                        //Hay equipos en el marcador
                                        //Hay que añadir un codigo que cuente los reintentos de cuando no se puede
                                        //encontrar datos ni en la franja y ni el marcador
                                        //y un codigo que vigile si hay cambios de equipos en el marcador
                                        //se comparan hasta que 3 veces coincidan y asi sabremos que el partido
                                        //termino y se inicio con otros equipos
                                        if (CapturaMarcador[0] != -1)
                                        {
                                            //Hay local
                                            intentos_de_captura_fallidos = 0;
                                            Equipo_L = CapturaMarcador[0];
                                            if (canciones_Local.Count == 0)
                                            {
                                                Cargar_Canciones(Equipo_L.ToString(), 1);
                                            }
                                            Goles_L = CapturaMarcador[2];
                                            inxtl = CapturaMarcador[5];
                                        }
                                        if (CapturaMarcador[1] != -1)
                                        {
                                            //Hay visitante
                                            intentos_de_captura_fallidos = 0;
                                            Equipo_V = CapturaMarcador[1];
                                            if (canciones_Visitante.Count == 0)
                                            {
                                                Cargar_Canciones(Equipo_V.ToString(), 2);
                                            }
                                            Goles_V = CapturaMarcador[3];
                                            inxtv = CapturaMarcador[6];
                                        }
                                        if (CapturaMarcador[4] != -1)
                                        {
                                            Tiempo_Juego = CapturaMarcador[4];
                                        }
                                        if (DevelopersLog())
                                        {
                                            // "JUN-BUC 0-0 [90'] 9:37AM, *"
                                            WriteLog(abreviaturasEquipos[inxtl] + "-" + abreviaturasEquipos[inxtv] + " " + Goles_L + "-" + Goles_V + " [" + TiempoJUEGO + "'] " + DateTime.Now.ToString());
                                        }
                                        //Comprobamos que almenos alla canciones del equipo

                                        if ((canciones_Local.Count > 0) || (canciones_Visitante.Count > 0))
                                        {
                                            // Si los equipos tienen canciones
                                            // Podemos hacer algo tomando en cuenta las variables
                                            reproducir_siguiente_one = true;
                                        }
                                        else
                                        {

                                            //   MessageBox.Show("No hay canciones");
                                            CancelarTodo();
                                           
                                        }
                                    }
                                }

                            }
                            else if((tl != -1) || (tv != -1))
                            {
                                //Encontramos almenos un equipo
                                //Sacamos el split de guion para obtener los goles
                                intentos_de_captura_fallidos = 0;
                                String[] marcador = captura_obtenida[1].Split('-');

                                if (tl != -1)
                                {

                                    if (tl != -1)
                                        if ((canciones_Local.Count == 0) || (tl != Equipo_L))
                                        {
                                            Equipo_L = tl;
                                            Cargar_Canciones(Equipo_L.ToString(), 1);
                                        }
                                    if (marcador.Length > 1)
                                    {
                                        int i;
                                        if (int.TryParse(marcador[0], out i))
                                        {
                                            Goles_L = i;
                                        }
                                    }
                                }
                                if (tv != -1)
                                {
                                    Equipo_V = tv;
                                    if ((canciones_Visitante.Count == 0) || (tv != Equipo_V))
                                    {
                                        Cargar_Canciones(Equipo_V.ToString(), 2);
                                    }
                                    if (marcador.Length > 1)
                                    {
                                        int i;
                                        if (int.TryParse(marcador[1], out i))
                                        {
                                            Goles_V = i;
                                        }
                                    }
                                }
                                //Como iniciamos desde la franja debemos leer el tiempo de
                                //juego y convertirlo a int solo minutos los segundos no nos interesan
                                //Tiempo_Juego = -1;
                                int iii;
                                if (int.TryParse(captura_obtenida[0].Split(':')[0], out iii))
                                {
                                    if(iii != -1) { Tiempo_Juego = iii;}  
                                }
                                // Si los equipos tienen canciones las reproducimos si no cancelamos todo
                                // y reiniciamos las variables
                                WriteLog(abreviaturasEquipos[inxtl] + "-" + abreviaturasEquipos[inxtv] + " " + Goles_L + "-" + Goles_V + " [" + TiempoJUEGO + "'] " + DateTime.Now.ToString());
                                
                                if ((canciones_Local.Count > 0) || (canciones_Visitante.Count > 0))
                                {
                                    //Reproducimos canciones tomando en cuenta las variables
                                    reproducir_siguiente_one = true;
                                    if (Tiempo_Juego == 0)
                                    {
                                        //Reproducimos canciones tomando en cuenta las variables
                                        reproducir_siguiente_one = false;
                                        ReproducirHimnos();
                                    }
                                    else if (Tiempo_Juego > 45)
                                    {
                                        himnos_reproducidos = false;
                                    }

                                }
                                else
                                {
                                    CancelarTodo();
                                }
                            }else
                            {
                                //No encontramos ningun equipo
                                intentos_de_captura_fallidos++;
                            }
                            if (reproducir_siguiente_one == true)
                            {

                                //Tomamos desiciones acorde a los valores Tiempo, EquipoL, GolesEL, EQUIPOV, GolesEV 
                                //Ultima reproduccion etc
                                if (reproduciendo == false)
                                {
                                    //Probabilidad de reproducir una cancion 80%
                                    var seed = Environment.TickCount;
                                    var random = new Random(seed);
                                    var value = random.Next(0, 9);

                                    if (value <= 7)
                                    {
                                        //Si no se esta reproduciendo nada colocamos nuestra cancion
                                        //La ultima reproducion fue de LastTeamsong
                                        if ((LastTeamsong == Equipo_V) || (Equipo_V == -1) || (LastTeamsong == -1))
                                        {

                                            //la ultima fue del visitante ahora un de local
                                            //Escogemos una aleatoria
                                            //Siempre es posible que salga un audio neutral
                                            var valueb = random.Next(0, 99);
                                            // MessageBox.Show("L"+valueb.ToString());
                                            if ((valueb > 0) && (valueb < 50) && (cancionespr_Local.Count > 0))
                                            {
                                                //Probabilidad de un audio dedicado al rival 50%
                                                var seed_n = Environment.TickCount;
                                                var random_n = new Random(seed_n);
                                                var value_n = random.Next(0, cancionespr_Local.Count - 1);
                                                ReproducirAudio(Equipo_L, cancionespr_Visitante[value_n]);
                                                cancionespr_Visitante.RemoveAt(value_n);
                                            }
                                            else if ((valueb > 49 && valueb < 60) && (canciones_neutrales.Count > 0))
                                            {
                                                //Probabildiad de un audio neutral corto 10%;
                                                var seed_n = Environment.TickCount;
                                                var random_n = new Random(seed_n);
                                                var value_n = random.Next(0, canciones_neutrales.Count - 1);
                                                ReproducirAudio(0, canciones_neutrales[value_n]);
                                                canciones_neutrales.RemoveAt(value_n);
                                            }
                                            else if (canciones_Local.Count > 0)
                                            {
                                                //Probabilidad de un audio normal del equipo 30%
                                                //  MessageBox.Show("aqui es");
                                                var seed_n = Environment.TickCount;
                                                var random_n = new Random(seed_n);
                                                var value_n = random.Next(0, canciones_Local.Count - 1);
                                                ReproducirAudio(Equipo_L, canciones_Local[value_n]);
                                                canciones_Local.RemoveAt(value_n);
                                            }
                                            LastTeamsong = Equipo_L;
                                        }
                                        else if (Equipo_V != -1)
                                        {
                                            //la ultima fue del local ahora una del visitante o si todavia no se ha 
                                            //reproducido nada empezamos con una del local
                                            //Escogemos una aleatoria
                                            //Siempre es posible que salga un audio neutral
                                            var valueb = random.Next(0, 99);
                                            //  MessageBox.Show("V" + valueb.ToString());
                                            if ((valueb > 0) && (valueb < 50) && (cancionespr_Visitante.Count > 0))
                                            {
                                                //Probabilidad de un audio dedicado al rival 50%
                                                var seed_n = Environment.TickCount;
                                                var random_n = new Random(seed_n);
                                                var value_n = random.Next(0, cancionespr_Visitante.Count - 1);
                                                ReproducirAudio(Equipo_V, cancionespr_Visitante[value_n]);
                                                cancionespr_Visitante.RemoveAt(value_n);
                                            }
                                            else if ((valueb > 49) && (valueb < 60) && (canciones_neutrales.Count > 0))
                                            {
                                                //Probabildiad de un audio neutral corto 10%;
                                                var seed_n = Environment.TickCount;
                                                var random_n = new Random(seed_n);
                                                var value_n = random.Next(0, canciones_neutrales.Count - 1);
                                                ReproducirAudio(0, canciones_neutrales[value_n]);
                                                canciones_neutrales.RemoveAt(value_n);
                                            }
                                            else if (canciones_Visitante.Count > 0)
                                            {
                                                //Probabilidad de un audio normal del equipo 30%
                                                var seed_n = Environment.TickCount;
                                                var random_n = new Random(seed_n);
                                                var value_n = random.Next(0, canciones_Visitante.Count - 1);
                                                ReproducirAudio(Equipo_V, canciones_Local[value_n]);
                                                canciones_Local.RemoveAt(value_n);
                                            }
                                            LastTeamsong = Equipo_V;
                                        }

                                        //Si el numero de canciones cargadas bajan del 20% en alguna de las 3 categorias
                                        // Canciones de equipo Local, Visitante, Canciones Neutrales
                                        //Si es 0 entonces se cancela directamente debido a que solo hay una 
                                        //y no aguanta repetir solamente 1 varias veces
                                        int mini_suma = canciones_Local.Count + cancionespr_Local.Count;
                                        if ((mini_suma < Linferior_CL) && (mini_suma > 0))
                                        {
                                            //Restaurar canciones Local
                                            canciones_Local = canciones_Local_Respaldo;
                                            cancionespr_Local = cancionespr_Local_Respaldo;
                                        }
                                        mini_suma = canciones_Visitante.Count + cancionespr_Visitante.Count;
                                        if ((mini_suma < Linferior_CV) && (mini_suma > 0))
                                        {
                                            //Restaurar canciones visitante
                                            canciones_Visitante = canciones_Visitante_Respaldo;
                                            cancionespr_Visitante = cancionespr_Visitante_Respaldo;
                                        }
                                        mini_suma = canciones_neutrales.Count;
                                        if ((mini_suma < Linferior_CN) && (mini_suma > 0))
                                        {
                                            //Restaurar canciones neutral
                                            canciones_neutrales = canciones_neutrales_respaldo;
                                        }
                                    }
                                }
                                //y reproducimos una cancion
                            }
                        }
                        else
                        {
                            WriteLog("No se obtuvo nada en captura");
                        }
                    }
                }
                else
                {

                    CancelarTodo();
                }
            }
            else
            {
               /* if (DevelopersLog())
                {
                    // "JUN-BUC 0-0 [90'] 9:37AM, *"
                    developerlabel.Text = "Se esta reproduciendo un audio";
                }*/
            }
        }
        public void Analizar_Marcador()
        {
            // Analizamos la imagen del marcador y devolvemos un array de variables para que sea analizado
            
        }
        #endregion

        #region Cargar canciones
        public void Cargar_Canciones_Neutrales()
        {
            string ruta = @"./audios/0";
            //Equipo local
            canciones_neutrales.Clear();
            if (Directory.Exists(ruta))
            {
                //guardamos las canciones encontradas
                var canciones_here = new DirectoryInfo(ruta);
                foreach (var song in canciones_here.GetFiles("*.mp3"))
                {
                    canciones_neutrales.Add(song.Name.ToLower());
                }
            }
            //Establecemos el limite inferior que es el 20%
            decimal prl = canciones_neutrales.Count / 100 * 2;
            Linferior_CN = Convert.ToInt32(Math.Floor(prl));
            if (Linferior_CN < 1) { Linferior_CN = 1; }
            //Hacemos respaldo
            canciones_neutrales_respaldo = canciones_neutrales;
        }
        public void Cargar_Canciones(String team_id, int lorv)
        {
            string ruta = @"./audios/" + team_id;
            if (lorv == 1)
            {
                //Equipo local
                canciones_Local.Clear();
                if (Directory.Exists(ruta))
                {
                    //guardamos las canciones encontradas
                    var canciones_here = new DirectoryInfo(ruta);
                    foreach (var song in canciones_here.GetFiles("*.mp3"))
                    {
                        //No debemos incluir aquellas que tengan una id en el nombre distinta a la del rival
                        //Recordar que una cancion puede que tenga varias id
                        String[] argus = song.Name.ToLower().Split('.');
                        int evitar = -1;
                        Boolean lista_separada = false;
                        if (argus.Length > 2)
                        {
                            for (int a = 0; a < argus.Length - 1; a++)
                            {
                                int i;

                                if (int.TryParse(argus[a], out i))
                                {
                                    if (i == Equipo_V)
                                    {
                                        lista_separada = true;
                                        evitar = 2;
                                    }
                                    else if (i != Equipo_V)
                                    {
                                        if (evitar == -1)
                                        {
                                            evitar = 1;
                                        }
                                    }
                                }
                            }
                        }
                        if (evitar != 1)
                        {
                            if(lista_separada == false)
                            {
                                canciones_Local.Add(song.Name);
                            }
                            else
                            {
                                cancionespr_Local.Add(song.Name);
                            }
                            
                        }
                        
                    }
                }
                //Establecemos el limite inferior que es el 20%
                decimal prl = (canciones_Local.Count + cancionespr_Local.Count)/100*2;
                Linferior_CL = Convert.ToInt32(Math.Floor(prl));
                if(Linferior_CL < 1) { Linferior_CL = 1; }
                //Hacemos los Respaldos
                canciones_Local_Respaldo = canciones_Local;
                cancionespr_Local_Respaldo = cancionespr_Local;
            }
            else if (lorv == 2)
            {
                //Equipo visitante
                canciones_Visitante.Clear();
                if (Directory.Exists(ruta))
                {
                    //guardamos las canciones encontradas
                    var canciones_here = new DirectoryInfo(ruta);
                    foreach (var song in canciones_here.GetFiles("*.mp3"))
                    {
                        //No debemos incluir aquellas que tengan una id en el nombre distinta a la del rival
                        //Recordar que una cancion puede que tenga varias id
                        String[] argus = song.Name.ToLower().Split('.');
                        int evitar = -1;
                        Boolean lista_separada = false;
                        if (argus.Length > 2)
                        {
                            for (int a = 0; a < argus.Length - 1; a++)
                            {
                                int i;

                                if (int.TryParse(argus[a], out i))
                                {
                                    if (i == Equipo_L)
                                    {
                                        evitar = 2;
                                    }
                                    else if (i != Equipo_L)
                                    {
                                        if (evitar == -1)
                                        {
                                            evitar = 1;
                                        }
                                    }
                                }
                            }
                        }
                        if (evitar != 1)
                        {
                            if (lista_separada == false)
                            {
                                canciones_Visitante.Add(song.Name);
                            }
                            else
                            {
                                cancionespr_Visitante.Add(song.Name);
                            }
                        }
                        
                    }
                }
                //Establecemos el limite inferior que es el 20%
                decimal prl = (canciones_Visitante.Count + cancionespr_Visitante.Count) / 100 * 2;
                Linferior_CV = Convert.ToInt32(Math.Floor(prl));
                if (Linferior_CV < 1) { Linferior_CV = 1; }
                //Respaldos
                canciones_Visitante_Respaldo = canciones_Visitante;
                cancionespr_Visitante_Respaldo = cancionespr_Visitante;
            }
            
        }
        #endregion

        #region Solicitar Canciones
        public List<(int, String)> SolicitarCanciones(String atr, int Equipo)
        {
            /*
             * Int equipo: 
             * 0 = Local, visitantes y neutral
             * 1 = local
             * 2 = visitante
             * 3 = Local y visitante
             * 10 = local y neutral
             * 20 = visitante y neutral
             * 
             * Atributos
             */
           
            List<(int, String)> retorno = new List<(int, String)>();
            int Hacia_otro_equipo = -1;
            Boolean Himno = false;
            int loovi = -1;
            int situacion = -1;
            Boolean tambien_empate = false;
            int Requerimientos = 0;
            if(atr != "") {
                String[] atributos = atr.Split(',');
                foreach (String atributo in atributos)
                {
                    //Comprobamos que no este buscando un himno
                    if ((atributo == "h") && (!Himno))
                    {
                        Himno = true;
                        Requerimientos++;
                    }
                    //Comprobamos que no se este solicitando un cantico contra otro equipo
                    if (Hacia_otro_equipo == -1)
                    {
                        int i;
                        if (int.TryParse(atributo, out i))
                        {
                            Hacia_otro_equipo = i;
                            Requerimientos++;
                        }
                    }
                    //Comprobamos si existe un atributo L o V que indique que el audio solo se reproduzca
                    //bajo esa condicion
                    if (loovi == -1)
                    {
                        if (atributo == "l")
                        {
                            loovi = 1;
                            Requerimientos++;
                        }
                        else if (atributo == "v")
                        {
                            loovi = 2;
                            Requerimientos++;
                        }
                    }
                    //Comprobamos si existe un atributo G, P o E que indique que el audio solo se reproduzca
                    //bajo esa condicion
                    if (situacion == -1)
                    {
                        if (atributo == "g")
                        {
                            situacion = 1;
                            Requerimientos++;
                        }
                        else if (atributo == "p")
                        {
                            situacion = 2;
                            Requerimientos++;
                        }
                    }
                    if ((atributo == "e") && (!tambien_empate))
                    {
                        tambien_empate = true;
                        Requerimientos++;
                    }
                }
            }
            //Recorremos todas las canciones
            //Audio del local
            List<String> Lista_combinada = new List<string>();
            (int s,int e) rangoL = (0,0);
            (int s, int e) rangoV = (0, 0);
            (int s, int e) rangoN = (0, 0);
            if ((Equipo == 0) || (Equipo == 1) || (Equipo == 10)) {
                Lista_combinada = Lista_combinada.Concat(canciones_Local).ToList();
                rangoL = (0,canciones_Local.Count-1);
            }
            //Audios del Visitante
            if ((Equipo == 0) || (Equipo == 2) || (Equipo == 20)){
                Lista_combinada = Lista_combinada.Concat(canciones_Visitante).ToList();
                rangoV = (Lista_combinada.Count-1, (Lista_combinada.Count - 1 +canciones_Visitante.Count - 1));
            }
            //Audios Neutrales
            if ((Equipo == 0) || (Equipo == 10) || (Equipo == 20))
            {
                Lista_combinada = Lista_combinada.Concat(canciones_neutrales).ToList();
                rangoN = (Lista_combinada.Count - 1, (Lista_combinada.Count - 1 + canciones_neutrales.Count - 1));
            }
            for(int SS = 0; SS <= Lista_combinada.Count-1; SS++)
            {
                String cancion = Lista_combinada[SS];
                String[] attr_cancion = cancion.Split('.');
                int Continuar = 0;
                for (int s = 0; s <= attr_cancion.Length - 2; s++)
                {
                    //Prueba A
                    if (Himno == true)
                    {
                        if (attr_cancion[s] == "h")
                        {
                            Continuar++;
                        }
                    }

                    //Prueba B
                    if (Hacia_otro_equipo != -1)
                    {
                        int i;
                        if (int.TryParse(attr_cancion[s], out i))
                        {
                            if (i == Hacia_otro_equipo)
                            {
                                Continuar++;
                            }
                        }
                    }

                    //Prueba C
                    if ((loovi == 1) && (attr_cancion[s] == "l"))
                    {
                        Continuar++;
                    }
                    else if ((loovi == 2) && (attr_cancion[s] == "v"))
                    {
                        Continuar++;
                    }
                    //Prueba D
                    if ((tambien_empate) && (attr_cancion[s] == "e"))
                    {
                        Continuar++;
                    }
                    //Prueba E
                    if ((situacion == 1) && (attr_cancion[s] == "g"))
                    {
                        Continuar++;
                    }
                    else if ((situacion == 2) && (attr_cancion[s] == "p"))
                    {
                        Continuar++;
                    }
                }

                if(Continuar >= Requerimientos)
                {
                    //Agregamos la cancion a retorno
                    if((SS >= rangoL.s) && (SS < rangoL.e+1))
                    {
                        //El audio es del local
                        retorno.Add((Equipo_L, Lista_combinada[SS]));
                    }else if ((SS >= rangoV.s) && (SS < rangoV.e + 1))
                    {
                        //El audio es del visitante
                        retorno.Add((Equipo_V, Lista_combinada[SS]));
                    }
                    else if((SS >= rangoN.s) && (SS < rangoN.e + 1))
                    {
                        //El audio es neutral
                        retorno.Add((0, Lista_combinada[SS]));
                    }
                     //retorno.Add((cancion.),())
                }
            }
            return retorno;
        }
        #endregion

        #region Reproducir Canciones
            #region Reproducir Himnos
            public void ReproducirHimnos()
            {
            /*
             * 
             * Esta Funcion Reproduce uno de los himno del local, si el local no tienen himno reproduce el del visitante
             * Int equipo: 
             * 0 = Local, visitantes y neutral
             * 1 = local
             * 2 = visitante
             * 3 = Local y visitante
             * 10 = local y neutral
             * 20 = visitante y neutral
            * 
            * Atributos
            */
            //Aqui hay que hacer algo con la estructura de datos.
            // string ruta = @"./audios/" + team_id;
           
            List<(int id,String name)> Himnos = SolicitarCanciones("h", 1);
                if (Himnos.Count == 0)
                {
                    //No hay himnos del local
                    Himnos = SolicitarCanciones("h", 2);
                    if (Himnos.Count > 0)
                    {
                    //hay himnos del visitante
                    //Escogemos uno al azar y lo reproducimos
                        var seed = Environment.TickCount;
                        var random = new Random(seed);
                        var value = random.Next(0, Himnos.Count-1);
                        LastTeamsong = Himnos[value].id;
                        ReproducirAudio(LastTeamsong,Himnos[value].name);
                        WriteLog("Reproduciendo Himno del visitante");
                        himnos_reproducidos = true;
                       
                    }//No hay himnos del local ni del visitante visitante 
                }else if (Himnos.Count > 0)
                {
                //Hay himnos del local
                //Escogemos uno al alzar y lo reproducimos
                    var seed = Environment.TickCount;
                    var random = new Random(seed);
                    var value = random.Next(0, Himnos.Count - 1);
                    LastTeamsong = Himnos[value].id;
                    ReproducirAudio(LastTeamsong,Himnos[value].name);
                    WriteLog("Reproduciendo Himno del local");
                    himnos_reproducidos = true;
                }

            }
            
            #endregion
            #region Reproducir Audio Exacto
            public void ReproducirAudio(int Teamid,String song)
            {
                reproduciendo = true;
                WriteLog("Reproducir = " + song);
                String ruta = @"./audios/" + Teamid.ToString() + "/" + song;
                backgroundAudio.RunWorkerAsync(argument: ruta); 
            }
            private void backgroundAudio_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
            {
                String ruta = (String)e.Argument;
                using (var audioFile = new AudioFileReader(ruta))
                {
                    using (outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Init(audioFile); outputDevice.Play();
                        
                        outputDevice.PlaybackStopped += reprofalse;
                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }
        }
        #endregion
            #region Dar aliento
        public void DarAliento(int team)
            {
                //team = 1 = local
                //team = 2 = visitante
                //Aqui hay que hacer algo con la estructura de datos.
            }
            #endregion
            void reprofalse(object sender, EventArgs e)
            {  
                reproduciendo = false;
                MessageBox.Show("Audio Terminado");
            }
        #endregion

        #region Reproducir una cancion Basico solo para test debe mejorarse
        public void Reproducir_Canciones()
        {
            List<String> canciones_disponibles = new List<string>();
            foreach (String team_id in ids_equipos_detectados)
            {
                string ruta = @"./audios/" + team_id;

                if (Directory.Exists(ruta))
                {
                    //guardamos las canciones encontradas
                    var canciones_here = new DirectoryInfo(ruta);
                    foreach (var song in canciones_here.GetFiles("*.mp3"))
                    {
                        canciones_disponibles.Add(ruta + "/" + song.Name);
                    }

                }
            }
            //Reproducimos la primera cancion
            using (var audioFile = new AudioFileReader(canciones_disponibles[0]))
            {
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile); outputDevice.Play();
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
           
        }
        #endregion

        public Bitmap CropImage(Bitmap source, System.Drawing.Rectangle section)
        {
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            Graphics g = Graphics.FromImage(bmp);

            g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);

            return bmp;
        }
        public List<String> RemoveLineEndings_split(string value)
        {
            List<String> completo = new List<string>();
            if (string.IsNullOrEmpty(value))
            {
                return completo;
            }
            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();
            String[] some = value.Replace("\r\n", "$").Replace("\n", "$").Replace("\r", "$").Replace(lineSeparator, "$").Replace(paragraphSeparator, "$").Split('$');
            for(int x = 0; x <= some.Length-1; x++)
            {
                var actxt = some[x].Replace(" ", "");
                if (actxt.Length > 0)
                {
                    completo.Add(some[x]);
                }
            }
            return completo;
        }


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct Rect
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
          //  public static extern bool AdjustWindowRect(Rect lpRect, uint dwStyle, bool bMenu);

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

           

        }
        const int SW_SHOWMINNOACTIVE = 7;

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        static void MinimizeWindow(IntPtr handle)
        {
            ShowWindow(handle, SW_SHOWMINNOACTIVE);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsIconic(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        // DLL libraries used to manage hotkeys
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

       
        #region Procesos que se usan pero que dejaran de hacerlo en un futuro
        private Bitmap InvertirBinaria(Bitmap source)
        {
            //create a blank bitmap the same size as original
            Bitmap bmpDest = new Bitmap(source.Width, source.Height);

            for (int i = 0; i < source.Width; i++)
            {
                for (int e = 0; e < source.Height; e++)
                {
                    // Color del pixel
                    System.Drawing.Color col = source.GetPixel(i, e);
                  
                    // Escala de grises
                   
                    byte value = 0;
                    if (col.B == 0)
                    {
                        value = 255;
                    }

                    System.Drawing.Color newColor = System.Drawing.Color.FromArgb(value, value, value);
                    //MessageBox.Show(col.B.ToString()+" - "+ newColor.B.ToString());
                    bmpDest.SetPixel(i, e, newColor);
                    // Asginar nuevo color


                }
            }

                return bmpDest;
        }
     
        public string marcador_a(String valor)
        {
           
           
            String prn = "";
            if (valor.Length > 0)
            {
                String a = valor.Replace(" ", "");
                for (int i = a.Length - 1; i >= 0; i--)
                {
                    String b = a.Substring(i, 1);
                    if ((IsNumeric(b)) && (prn == ""))
                    {
                        prn = b;
                    }
                    else if ((IsNumeric(b)) && (prn != ""))
                    {
                        prn = b + prn;
                    }
                    else if ((!IsNumeric(b)) && (prn != ""))
                    {
                        i = -1;
                    }
                }
            }
            return prn;
        }
        public string marcador_b(String valor)
        {
           
            String prn = "";
            if (valor.Length > 0)
            {
                String a = valor.Replace(" ", "");
                for (int i = 0; i < a.Length; i++)
                {
                    String b = a.Substring(i, 1);
                    if ((IsNumeric(b)) && (prn == ""))
                    {
                        prn = b;
                    }
                    else if ((IsNumeric(b)) && (prn != ""))
                    {
                        prn += b;
                    }
                    else if ((!IsNumeric(b)) && (prn != ""))
                    {
                        i = a.Length;
                    }
                }
            }
            return prn;
        }
        public bool IsNumeric(object Expression)

        {

            bool isNum;

            double retNum;

            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);

            return isNum;

        }
        public Bitmap BinaryImage_b(Bitmap source, int umb)
        {
            // Bitmap con la imagen binaria
            Bitmap target = new Bitmap(source.Width, source.Height, source.PixelFormat);
            // Recorrer pixel de la imagen
            for (int i = 0; i < source.Width; i++)
            {
                for (int e = 0; e < source.Height; e++)
                {
                    // Color del pixel
                    System.Drawing.Color col = source.GetPixel(i, e);
                    // Escala de grises
                    var z = ((col.R + col.G + col.B) / 3);
                    byte value = 0;
                    if (col.B > (col.R + col.G - 20))
                    {

                    }else if (col.G > (col.R + col.B - 20))
                    {

                    }else if ((z > (col.R-55)) && (col.R > 150) && (col.G > 150) && (col.B > 150))
                    {
                        
                        value = 255;
                    }

                    // Asginar nuevo color
                    System.Drawing.Color newColor = System.Drawing.Color.FromArgb(value, value, value);
                    target.SetPixel(i, e, newColor);

                }
            }

            /*    for (int e = 0; e < source.Height; e++)
                {
                    var black = 1;
                    var white = 1;
                    for (int i = 0; i < source.Width; i++)
                    {
                        Color col = target.GetPixel(i, e);
                        if(col.R == 255)
                        {
                            white += 1;
                        }else if (col.R == 0)
                        {
                            black += 1;
                        }
                    }
                    //   MessageBox.Show(black.ToString() + " - " + source.Width);
                    decimal azn = (decimal)black / (decimal)source.Width;
                    if (azn < (decimal)0.7)
                    {
                       // MessageBox.Show(black.ToString() + " - " + source.Width + " [ "+ (black / source.Width).ToString());
                        for (int i = 0; i < source.Width; i++)
                        {
                            target.SetPixel(i, e, System.Drawing.Color.FromArgb(0, 0, 0));

                        }
                        target.SetPixel(0, e, System.Drawing.Color.FromArgb(0, 255, 0));
                    }
        }*/

            return target;
        }
        public Bitmap BinaryImage(Bitmap source, int umb)
        {
            // Bitmap con la imagen binaria
            Bitmap target = new Bitmap(source.Width, source.Height, source.PixelFormat);
            // Recorrer pixel de la imagen
            for (int i = 0; i < source.Width; i++)
            {
                for (int e = 0; e < source.Height; e++)
                {
                    // Color del pixel
                    System.Drawing.Color col = source.GetPixel(i, e);
                    // Escala de grises
                    byte gray = (byte)(col.R * 0.3f + col.G * 0.59f + col.B * 0.11f);
                    
                    // Blanco o negro
                    byte value = 0;
                    if (gray > umb)
                    {
                        value = 255;
                    }
                    // Asginar nuevo color
                    System.Drawing.Color newColor = System.Drawing.Color.FromArgb(value, value, value);
                    target.SetPixel(i, e, newColor);

                }
            }

            return target;
        }

        #endregion

        #region Codigo necesario para las capturas

        #region necesario para start
        private byte[] _previousScreen;
        private bool _run, _init;
        public int Size { get; private set; }

       
        int widthdx;
        int heightdx;
        double w_p_i;
        double h_p_i;
        System.Drawing.Rectangle FRANJAMENUSTART;
        System.Drawing.Rectangle TiempoJUEGO;
        System.Drawing.Rectangle MARCADOR_FRANJA;
        System.Drawing.Rectangle IMGEQUIPOLOCAL;
        System.Drawing.Rectangle EQUIPOVISITANTE;
        System.Drawing.Rectangle MAGMARCADOR;
        System.Drawing.Rectangle NOMBRESCI;
        SharpDX.Direct3D11.Device device;
        SharpDX.DXGI.Output output;
        SharpDX.DXGI.Output1 output1;
        Texture2DDescription textureDesc;
        public void PreStart()
        {


            var  factory = new Factory1();
            //Get first adapter
            var adapter = factory.GetAdapter1(0);
            //Get device from adapter
            device = new SharpDX.Direct3D11.Device(adapter);
            //Get front buffer of the adapter
            output = adapter.GetOutput(0);
            output1 = output.QueryInterface<Output1>();

            // Width/Height of desktop to capture
            widthdx = output.Description.DesktopBounds.Right;
            heightdx = output.Description.DesktopBounds.Bottom;

            // Create Staging texture CPU-accessible
             textureDesc = new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = widthdx,
                Height = heightdx,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            };
           
            
        }
        public void Calibrar_secciones()
        {
            w_p_i = 0.01 * (H_r - H_l);
            h_p_i = 0.01 * (H_b - H_t);

            FRANJAMENUSTART = new System.Drawing.Rectangle(Convert.ToInt32(13 * w_p_i), 0, Convert.ToInt32(74 * w_p_i), Convert.ToInt32(18 * h_p_i));
            TiempoJUEGO = new System.Drawing.Rectangle(Convert.ToInt32(46 * w_p_i), Convert.ToInt32(6 * h_p_i), Convert.ToInt32(8 * w_p_i), Convert.ToInt32(4.3 * h_p_i));
            MARCADOR_FRANJA = new System.Drawing.Rectangle(Convert.ToInt32(41.5 * w_p_i), Convert.ToInt32(9 * h_p_i), Convert.ToInt32(17.5 * w_p_i), Convert.ToInt32(7.2 * h_p_i));
            IMGEQUIPOLOCAL = new System.Drawing.Rectangle(Convert.ToInt32(13 * w_p_i), Convert.ToInt32(8.7 * h_p_i), Convert.ToInt32(28.4 * w_p_i), Convert.ToInt32(8.7 * h_p_i));
            EQUIPOVISITANTE = new System.Drawing.Rectangle(Convert.ToInt32(58.5 * w_p_i), Convert.ToInt32(8.7 * h_p_i), Convert.ToInt32(28.4 * w_p_i), Convert.ToInt32(8.7 * h_p_i));
            MAGMARCADOR = new System.Drawing.Rectangle(Convert.ToInt32(0.8 * w_p_i), Convert.ToInt32(1.4 * h_p_i), Convert.ToInt32(26 * w_p_i), Convert.ToInt32(6 * h_p_i));
            NOMBRESCI = new System.Drawing.Rectangle(Convert.ToInt32(5.85 * w_p_i), Convert.ToInt32(1.5 * h_p_i), Convert.ToInt32(80 * w_p_i), Convert.ToInt32(5 * h_p_i));
        }
        #endregion

        public List<String> Obtener_captura(System.Diagnostics.Process proc)
        {
           
            IntPtr wHnd = proc.MainWindowHandle;
            Boolean isMinimized = IsIconic(wHnd);


           List<String> bitmaps = new List<String>();
            //MessageBox.Show(isMinimized.ToString());
            if (isMinimized == true)
            {
                //No Hacemos la captura
                return bitmaps;
            }
          //  MessageBox.Show(GetForegroundWindow().ToString() + " - " + wHnd);
            if ((isMinimized == false) && (GetForegroundWindow() == wHnd))
            {
             /*CON GetForegroundWindow nos aseguramos que el juego no este minimizado
               ni escondido atras de ottra ventana (Y) */
             //Colocamos el Fifa arriba y capturamos la pantalla
             //   SetForegroundWindow(wHnd);

            var screenTexture = new Texture2D(device, textureDesc);
            Task.WaitAll( Task.Factory.StartNew(() =>
            {
                // Duplicate the output
                using (var duplicatedOutput = output1.DuplicateOutput(device))
                {
                    bool captureDone = false;
                    for (int i = 0; !captureDone; i++)
                    {
                        try
                        {
                            SharpDX.DXGI.Resource screenResource;
                            OutputDuplicateFrameInformation duplicateFrameInformation;

                            // Try to get duplicated frame within given time
                            duplicatedOutput.AcquireNextFrame(1, out duplicateFrameInformation, out screenResource);
                            
                            if (i > 0)
                            {
                                // copy resource into memory that can be accessed by the CPU
                                using (var screenTexture2D = screenResource.QueryInterface<Texture2D>())
                                    device.ImmediateContext.CopyResource(screenTexture2D, screenTexture);

                                // Get the desktop capture texture
                                var mapSource = device.ImmediateContext.MapSubresource(screenTexture, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);

                                // Create Drawing.Bitmap
                                var bitmap = new Bitmap(widthdx, heightdx, PixelFormat.Format32bppArgb);
                                var boundsRect = new System.Drawing.Rectangle(0, 0, widthdx, heightdx);

                                // Copy pixels from screen capture Texture to GDI bitmap
                                var mapDest = bitmap.LockBits(boundsRect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
                                var sourcePtr = mapSource.DataPointer;
                                var destPtr = mapDest.Scan0;
                                for (int y = 0; y < heightdx; y++)
                                {
                                    // Copy a single line 
                                    Utilities.CopyMemory(destPtr, sourcePtr, widthdx * 4);

                                    // Advance pointers
                                    sourcePtr = IntPtr.Add(sourcePtr, mapSource.RowPitch);
                                    destPtr = IntPtr.Add(destPtr, mapDest.Stride);
                                }

                                // Release source and dest locks
                                bitmap.UnlockBits(mapDest);
                                device.ImmediateContext.UnmapSubresource(screenTexture, 0);

                            
                                
                                // Send Message
                                //Main.Chat.AddMessage(null, "~b~Screenshot saved as " + filename);

                                // Capture done
                                captureDone = true;

                               
                                var rect = new User32.Rect();
                                    User32.GetWindowRect(proc.MainWindowHandle, ref rect);
                                    int widths = rect.right - rect.left ;
                                    int heights = rect.bottom - rect.top;
                                int cordx = 0; int cordy = 0;
                                if ((widths == widthdx) && (heights == heightdx))
                                {
                                    widths = widthdx;
                                    heights = heightdx;
                                }
                                else if ((widths != widthdx) || (heights != heightdx))
                                {
                                    //no FullScreen (tal vez)
                                    widths = rect.right - rect.left - SystemInformation.Border3DSize.Width * 2 - 20;
                                    heights = rect.bottom - rect.top - SystemInformation.Border3DSize.Height - 40 - SystemInformation.BorderSize.Height * 2;
                                    cordx = rect.left + SystemInformation.Border3DSize.Width + 10;
                                    cordy = rect.top + SystemInformation.CaptionHeight + SystemInformation.BorderSize.Height + 10;
                                }
                               
                                // Save the output 
                                //En el producto final no sera necesario guardar la imagen solo escanearla directamente.
                                //Recortar por el hud
                                Random rnd = new Random();
                                System.Drawing.Rectangle rectOrig = new System.Drawing.Rectangle(H_l, H_t, H_r - H_l, H_b - H_t);
                                bitmap = (Bitmap)bitmap.Clone(rectOrig, bitmap.PixelFormat);
                                String prefijo = rnd.Next(200).ToString();


                                // bitmap.Save("Imagenes/test"+ prefijo + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                #region Recortar     

                                //RECOTAR POR SECCIONES Y GUARDAR



                                //FRANJA MENU START (No se usa regularmente)
                                /* Bitmap fms = (Bitmap)bitmap.Clone(FRANJAMENUSTART, bitmap.PixelFormat);
                                 fms.Save("Imagenes/FS_test" + prefijo + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                 fms.Dispose();*/


                                

                                //Tiempo JUEGO
                                //bitmaps.Add((Bitmap)bitmap.Clone(TiempoJUEGO, bitmap.PixelFormat));
                                using (var engine = new TesseractEngine(@"./tessdata", "spa", EngineMode.Default, "bazaar"))
                                {
                                    engine.SetVariable("tessedit_char_whitelist", "01234567890:");
                                    using (var img = PixConverter.ToPix((Bitmap)bitmap.Clone(TiempoJUEGO, bitmap.PixelFormat)))
                                    {
                                        using (var page = engine.Process(img))
                                        {
                                            bitmaps.Add(page.GetText());
                                        }
                                    }
                                }
                                // Bitmap tj = (Bitmap)bitmap.Clone(TiempoJUEGO, bitmap.PixelFormat);
                                // tj.Save("Imagenes/TJ_test" + prefijo + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                // tj.Dispose();

                                //MARCADOR_FRANJA
                                //bitmaps.Add((Bitmap)bitmap.Clone(MARCADOR_FRANJA, bitmap.PixelFormat));
                                using (var engine = new TesseractEngine(@"./tessdata", "spa", EngineMode.Default, "bazaar"))
                                {
                                    engine.SetVariable("tessedit_char_whitelist", "0123456789-()");
                                    using (var img = PixConverter.ToPix((Bitmap)bitmap.Clone(MARCADOR_FRANJA, bitmap.PixelFormat)))
                                    {
                                        using (var page = engine.Process(img))
                                        {
                                            bitmaps.Add(page.GetText());
                                           
                                        }
                                    }
                                }
                                 Bitmap maf = (Bitmap)bitmap.Clone(MARCADOR_FRANJA, bitmap.PixelFormat);
                                 maf.Save("Imagenes/MAF_test" + prefijo + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                 maf.Dispose();

                                //IMGEQUIPOLOCAL
                                //bitmaps.Add((Bitmap)bitmap.Clone(IMGEQUIPOLOCAL, bitmap.PixelFormat));
                                using (var engine = new TesseractEngine(@"./tessdata", "spa", EngineMode.Default, "bazaar"))
                                {
                                    engine.SetVariable("tessedit_char_whitelist", "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz");
                                    using (var img = PixConverter.ToPix((Bitmap)bitmap.Clone(IMGEQUIPOLOCAL, bitmap.PixelFormat)))
                                    {
                                        using (var page = engine.Process(img))
                                        {
                                            bitmaps.Add(page.GetText());
                                        }
                                    }
                                }
                                // Bitmap loct = (Bitmap)bitmap.Clone(IMGEQUIPOLOCAL, bitmap.PixelFormat);
                                // loct.Save("Imagenes/LOC_test" + prefijo + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                // loct.Dispose();

                                //EQUIPO VISITANTE
                                //bitmaps.Add((Bitmap)bitmap.Clone(EQUIPOVISITANTE, bitmap.PixelFormat));
                                using (var engine = new TesseractEngine(@"./tessdata", "spa", EngineMode.Default, "bazaar"))
                                {
                                    engine.SetVariable("tessedit_char_whitelist", "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz");
                                    using (var img = PixConverter.ToPix((Bitmap)bitmap.Clone(EQUIPOVISITANTE, bitmap.PixelFormat)))
                                    {
                                        using (var page = engine.Process(img))
                                        {
                                            bitmaps.Add(page.GetText());
                                        }
                                    }
                                }
                                // Bitmap vitt = (Bitmap)bitmap.Clone(EQUIPOVISITANTE, bitmap.PixelFormat);
                                // vitt.Save("Imagenes/VIS_test" + prefijo + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                // vitt.Dispose();

                                //Este if nos ahorra unas imagenes en la memoria
                                if (Empezado == false)
                                {
                                    //NOMBRE DE EQUIPOs QUE APARECE EN CARGA INTERACTIVA
                                    //bitmaps.Add((Bitmap)bitmap.Clone(NOMBRESCI, bitmap.PixelFormat));
                                    using (var engine = new TesseractEngine(@"./tessdata", "spa", EngineMode.Default, "bazaar"))
                                    {
                                        engine.SetVariable("tessedit_char_whitelist", "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz-");
                                        using (var img = PixConverter.ToPix((Bitmap)bitmap.Clone(NOMBRESCI, bitmap.PixelFormat)))
                                        {
                                            using (var page = engine.Process(img))
                                            {
                                                bitmaps.Add(page.GetText());
                                            }
                                        }
                                    }
                                    //  Bitmap noci = (Bitmap)bitmap.Clone(NOMBRESCI, bitmap.PixelFormat);
                                    //  noci.Save("Imagenes/NOCI_test" + prefijo + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                    //  noci.Dispose();
                                }
                                else
                                {
                                    //MARCADOR
                                    //ESTE REQUERIRA UN MEJOR PROCESADO DE OCR ESO SERA OTRO PROYECTO
                                    //bitmaps.Add((Bitmap)bitmap.Clone(MAGMARCADOR, bitmap.PixelFormat));
                                    using (var engine = new TesseractEngine(@"./tessdata", "spa", EngineMode.Default, "bazaar"))
                                    {
                                        engine.SetVariable("tessedit_char_whitelist", "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz01234567890|:-");
                                        using (var img = PixConverter.ToPix((Bitmap)bitmap.Clone(MAGMARCADOR, bitmap.PixelFormat)))
                                        {
                                            using (var page = engine.Process(img))
                                            {
                                                /*
                                                 * 
                                                 * Debe devolver algo como: 'Idlocal,IdVisitante,GolesLocal,GolesVisitante,Minutos,indexLoval,IndexVisitante'
                                                 * Todo en valores numericos
                                                 * Eso lo haremos gracias a nuestro ocr mejorado ;)
                                                 */
                                                //  bitmaps.Add(page.GetText());
                                                bitmaps.Add(Equipo_L.ToString()+","+ Equipo_V.ToString() + ",0,0," + Tiempo_Juego.ToString()+","+index_Equipo_L.ToString()+","+index_Equipo_V.ToString());
                                                Tiempo_Juego += 5;
                                            }
                                        }
                                    }
                                    //MessageBox.Show(bitmaps.Count.ToString());
                                    // Bitmap mag = (Bitmap)bitmap.Clone(MAGMARCADOR, bitmap.PixelFormat);
                                    // mag.Save("Imagenes/MAG_test" + prefijo + ".png", System.Drawing.Imaging.ImageFormat.Png);
                                    // mag.Dispose();
                                }
                                    
                                     //Ya tenemos el Bitmap solo falta es escanear

                                     // Escanear("testpiczzyc.png");


            
                                #endregion
                            }

                            screenResource.Dispose();
                            duplicatedOutput.ReleaseFrame();

                        }
                        catch (SharpDXException e)
                        {
                            if (e.ResultCode.Code != SharpDX.DXGI.ResultCode.WaitTimeout.Result.Code)
                            {
                                throw e;
                            }
                        }
                    }

                }
                
            }));
               
                //   while (!_init) ;

            }

            /*  if(bitmaps.Count > 0)
              {*/
            // return Task.FromResult(bitmaps);
            return bitmaps;
           // }
            /*  else
              {
                  return Task.FromResult(bitmaps);
              }*/

        }

        #endregion
        private void Form1_Load(object sender, EventArgs e)
        {
          //  AllocConsole();
        }

       

    }
}
