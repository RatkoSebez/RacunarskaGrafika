// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;
using SharpGL.Enumerations;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        /// <summary>
        ///  Nabrojani tip OpenGL rezima filtriranja tekstura
        /// </summary>
        public enum TextureGenMode
        {
            ObjectLinear,
            EyeLinear,
            SphereMap
        };

        #region Atributi

        /// <summary>
        ///  Identifikator teksture
        /// </summary>
        uint[] textureIDs;

        /// <summary>
        ///  Izabrana OpenGL mehanizam za iscrtavanje.
        /// </summary>
        private TextureGenMode m_selectedMode = TextureGenMode.SphereMap;

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 7000.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        private float[] pointLinePolygonVertices = new float[]
        {
            4, 0f, 6,
            4, 0f, -6,
            -4, 0f, -6,
            -4, 0f, 6
        };

        private Cylinder cylinder, cylinder2, cylinder3;

        /// <summary>
        ///	 Referenca na OpenGL instancu unutar aplikacije
        /// </summary>
        private OpenGL gl;

        private Double goalHeight;

        private Double ballScale;

        private Boolean ballGoingUp;

        private Double ballHeight;

        private Double ballX;

        private Double ballY;

        private Double ballRotation;

        private Double ballRotationSpeed;

        private Boolean startAnimation1;

        private Boolean startAnimation2;

        private Boolean ballHitsGoal;

        private MainWindow window;

        #endregion Atributi

        #region Properties

        public TextureGenMode SelectedMode
        {
            get { return m_selectedMode; }
            set
            {
                m_selectedMode = value;

                // Projekciona ravan
                float[] zPlane = { 0.0f, 0.0f, 1.0f, 1.0f };

                switch (m_selectedMode)
                {
                    case TextureGenMode.ObjectLinear:
                        // Object Linear
                        gl.TexGen(OpenGL.GL_S, OpenGL.GL_TEXTURE_GEN_MODE, OpenGL.GL_OBJECT_LINEAR);
                        gl.TexGen(OpenGL.GL_T, OpenGL.GL_TEXTURE_GEN_MODE, OpenGL.GL_OBJECT_LINEAR);
                        gl.TexGen(OpenGL.GL_S, OpenGL.GL_OBJECT_PLANE, zPlane);
                        gl.TexGen(OpenGL.GL_T, OpenGL.GL_OBJECT_PLANE, zPlane);
                        break;

                    case TextureGenMode.EyeLinear:
                        // Eye Linear
                        gl.TexGen(OpenGL.GL_S, OpenGL.GL_TEXTURE_GEN_MODE, OpenGL.GL_EYE_LINEAR);
                        gl.TexGen(OpenGL.GL_T, OpenGL.GL_TEXTURE_GEN_MODE, OpenGL.GL_EYE_LINEAR);
                        gl.TexGen(OpenGL.GL_S, OpenGL.GL_EYE_PLANE, zPlane);
                        gl.TexGen(OpenGL.GL_T, OpenGL.GL_EYE_PLANE, zPlane);
                        break;

                    case TextureGenMode.SphereMap:
                        // Sphere Map
                        gl.TexGen(OpenGL.GL_S, OpenGL.GL_TEXTURE_GEN_MODE, OpenGL.GL_SPHERE_MAP);
                        gl.TexGen(OpenGL.GL_T, OpenGL.GL_TEXTURE_GEN_MODE, OpenGL.GL_SPHERE_MAP);
                        break;
                }
            }
        }


        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        public Double GoalHeight {
            get { return goalHeight; }
            set { goalHeight = value; }
        }

        public Double BallScale {
            get { return ballScale; }
            set { ballScale = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        public Double BallHeight
        {
            get { return ballHeight; }
            set { ballHeight = value; }
        }

        public Double BallRotationSpeed {
            get { return ballRotationSpeed; }
            set { ballRotationSpeed = value; }
        }

        public Boolean StartAnimation1 {
            get { return startAnimation1; }
            set { startAnimation1 = value; }
        }

        public Boolean StartAnimation2
        {
            get { return startAnimation2; }
            set { startAnimation2 = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;
            this.gl = gl;
            this.goalHeight = 3;
            this.ballScale = 1;
            textureIDs = new uint[1];
            this.ballHeight = 0;
            this.ballGoingUp = true;
            this.ballRotation = 0;
            this.ballRotationSpeed = 5;
            this.startAnimation1 = false;
            this.startAnimation2 = false;
            this.ballX = 0;
            this.ballY = 0;
            this.ballHitsGoal = false;
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.FrontFace(OpenGL.GL_CCW);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            SetupLighting(gl);
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_FLAT);
            m_scene.LoadScene();
            m_scene.Initialize();
            cylinder = new Cylinder();
            cylinder.BaseRadius = 0.1;
            cylinder.TopRadius = 0.1;
            cylinder.Height = goalHeight;
            cylinder.Slices = 100;
            cylinder.Stacks = 100;
            cylinder2 = new Cylinder();
            cylinder2.BaseRadius = 0.1;
            cylinder2.TopRadius = 0.1;
            cylinder2.Height = 2;
            cylinder2.Slices = 100;
            cylinder2.Stacks = 100;
            cylinder3 = new Cylinder();
            cylinder3.BaseRadius = 0.1;
            cylinder3.TopRadius = 0.1;
            cylinder3.Height = 1;
            cylinder3.Slices = 100;
            cylinder3.Stacks = 100;
        }

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(45f, (double)width / height, 0.5f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        /// <summary>
        /// Podesavanje osvetljenja
        /// </summary>
        private void SetupLighting(OpenGL gl)
        {
            float[] global_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);

            //tackasto svetlo
            float[] light0pos = new float[] { 0.0f, 3.0f, -10.0f, 1.0f };
            float[] light0ambient = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] light0diffuse = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            //float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            //gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);

            //reflektorsko svetlo
            float[] light1pos = new float[] { 0.0f, 2.0f, 0.0f, 1.0f };
            float[] light1ambient = new float[] { 1.0f, 0.0f, 0.6f, 1.0f };
            float[] light1diffuse = new float[] { 1.0f, 0.0f, 0.6f, 1.0f };
            float[] smer = new float[] { 0.0f, 0.0f, -1.0f };

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light1pos);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, light1ambient);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, light1diffuse);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, smer);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 30.0f);

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Enable(OpenGL.GL_LIGHT1);
            gl.Enable(OpenGL.GL_NORMALIZE);
        }

        public void EnableTexture(String imagePath, bool ok, bool ok2) {
            //gl.MatrixMode(OpenGL.GL_TEXTURE);
            //gl.LoadIdentity();
            //gl.Scale(500, 500, 500);
            //gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            if (ok) {
                gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            }
            else {
                gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);
            }

            gl.GenTextures(1, textureIDs);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureIDs[0]);
            // Ucitaj sliku i podesi parametre teksture
            Bitmap image = new Bitmap(imagePath);
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            BitmapData imageData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, (int)OpenGL.GL_RGBA8, image.Width, image.Height, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);

            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

            image.UnlockBits(imageData);
            image.Dispose();

            if (ok2)
            {
                // Ukljuci generisanje koord. teksture
                gl.Enable(OpenGL.GL_TEXTURE_GEN_S);
                gl.Enable(OpenGL.GL_TEXTURE_GEN_T);
            }
            else {
                gl.Disable(OpenGL.GL_TEXTURE_GEN_S);
                gl.Disable(OpenGL.GL_TEXTURE_GEN_T);
            }

            // sferno podrazumevani nacin generisanja koord. teksture
            // gl.TexGen(OpenGL.GL_S, OpenGL.GL_TEXTURE_GEN_MODE, OpenGL.GL_SPHERE_MAP);
            // gl.TexGen(OpenGL.GL_T, OpenGL.GL_TEXTURE_GEN_MODE, OpenGL.GL_SPHERE_MAP);
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();

            gl.PushMatrix();

            gl.Viewport(0, 0, m_width, m_height);
            gl.PointSize(1f);
            gl.LineWidth(1f);
            gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);

            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            gl.Rotate(20f, 1.0f, 0.0f, 0.0f);
            gl.Scale(500f, 500f, 500f);
            gl.Color(0.0f, 0.5f, 0.0f);

            EnableTexture("..//..//images//trava.jpg", false, false);

            //podloga
            gl.Begin(OpenGL.GL_QUADS);
            for (int i = 0; i < pointLinePolygonVertices.Length; i = i + 3)
            {
                if(i == 0) gl.TexCoord(0.0f, 0.0f);
                if (i == 3) gl.TexCoord(0.0f, 1.5f);
                if (i == 6) gl.TexCoord(1.0f, 1.5f);
                if (i == 9) gl.TexCoord(1.0f, 0.0f);
                gl.Vertex(pointLinePolygonVertices[i], pointLinePolygonVertices[i + 1], pointLinePolygonVertices[i + 2]);
            }
            gl.End();

            EnableTexture("..//..//images//bela-plastika.jpg", true, true);

            //visina gola po izboru
            cylinder.Height = goalHeight;

            // cilindri
            //gl.FrontFace(OpenGL.GL_CW);
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Rotate(-90f, 0f, 0f);
            gl.Translate(1f, 5f, 1f);
            cylinder.CreateInContext(gl);
            cylinder.Render(gl, RenderMode.Render);
            gl.Translate(-2f, 0f, 0f);
            cylinder.Render(gl, RenderMode.Render);
            gl.Rotate(0f, 90f, 0f);
            cylinder2.CreateInContext(gl);
            cylinder2.Render(gl, RenderMode.Render);
            gl.Rotate(0f, -90f, 0f);
            gl.Translate(1f, 0f, -1f);
            cylinder3.CreateInContext(gl);
            cylinder3.Render(gl, RenderMode.Render);
            gl.Translate(-1f, 0f, 1f);
            gl.Translate(0f, -10f, 0f);
            cylinder.Render(gl, RenderMode.Render);
            gl.Translate(2f, 0f, 0f);
            cylinder.Render(gl, RenderMode.Render);
            gl.Rotate(0f, -90f, 0f);
            cylinder2.Render(gl, RenderMode.Render);
            gl.Rotate(0f, 90f, 0f);
            gl.Translate(-1f, 0f, -1f);
            cylinder3.Render(gl, RenderMode.Render);
            gl.Translate(1f, 0f, 0f);

            EnableTexture("..//..//images//ball.jpg", false, true);

            //lopta
            gl.Translate(-1f, 2f, 0.14f);
            gl.Scale(0.25, 0.25, 0.25);
            //skaliranje lopte po zelji korisnika
            gl.Scale(ballScale, ballScale, ballScale);
            //animacija odskakanja lopte
            if (startAnimation1) AnimateBallJump();
            if (startAnimation2) AnimateBallScore();
            m_scene.Draw();
            gl.Scale(1 / ballScale, 1 / ballScale, 1 / ballScale);

            gl.PopMatrix();

            //tekst u gornjem desnom uglu
            gl.PushMatrix();
            //gl.Disable(OpenGL.GL_DEPTH_TEST);
            //gl.Viewport(2*m_width / 3, 2*m_height / 3, 2 * m_width / 3, 2 * m_height / 3);
            gl.Viewport(m_width - 160, m_height - 130, m_width - 160, m_height - 130);
            gl.DrawText(0, 130, 1.0f, 1.0f, 0.0f, "Tahoma", 10, "Predmet: Racunarska grafika");
            gl.DrawText(0, 130, 1.0f, 1.0f, 0.0f, "Tahoma", 10, "_______________________");
            gl.DrawText(0, 110, 1.0f, 1.0f, 0.0f, "Tahoma", 10, "Sk.god: 2021/22.");
            gl.DrawText(0, 110, 1.0f, 1.0f, 0.0f, "Tahoma", 10, "______________");
            gl.DrawText(0, 90, 1.0f, 1.0f, 0.0f, "Tahoma", 10, "Ime: Ratko");
            gl.DrawText(0, 90, 1.0f, 1.0f, 0.0f, "Tahoma", 10, "_________");
            gl.DrawText(0, 70, 1.0f, 1.0f, 0.0f, "Tahoma", 10, "Prezime: Sebez");
            gl.DrawText(0, 70, 1.0f, 1.0f, 0.0f, "Tahoma", 10, "____________");
            gl.DrawText(0, 50, 1.0f, 1.0f, 0.0f, "Tahoma", 10, "Sifra zad: 7.1");
            gl.DrawText(0, 50, 1.0f, 1.0f, 0.0f, "Tahoma", 10, "___________");
            // gl.DrawText(0, 0, 1.0f, 1.0f, 0.0f, "Tahoma", 10, "Vucicu pederu");
            gl.Viewport(0, 0, m_width, m_height);
            gl.PopMatrix();

            // Oznaci kraj iscrtavanja
            gl.Flush();
        }

        public void AnimateBallJump() {
            if (ballGoingUp)
            {
                ballHeight += 0.2;
                if (ballHeight > 3.0) ballGoingUp = false;
            }
            else
            {
                ballHeight -= 0.2;
                if (ballHeight < 0.1) ballGoingUp = true;
            }
            gl.Translate(0f, 0f, ballHeight);
            ballRotation += ballRotationSpeed;
            gl.Rotate(ballRotation, 1.0f, 0.0f, 0.0f);
        }

        public void AnimateBallScore()
        {
            window.IsEnabled = false;
            if (ballHitsGoal == false)
            {
                ballHeight += 0.2;
                ballX += 0.090;
                ballY -= 0.2;
                if (ballHeight > 6.7) ballHitsGoal = true;
            }
            else {
                ballHeight += 0.2;
                //ballX = -ballX;
                ballX -= 0.090;
                ballY -= 0.2;
                if (ballHeight > 10) {
                    ballHeight = 0;
                    ballX = 0;
                    ballY = 0;
                    ballHitsGoal = false;
                    startAnimation2 = false;
                    window.IsEnabled = true;
                }
            }
            gl.Translate(ballX, ballY, ballHeight);
        }

        public void Window(MainWindow window){
            this.window = window;
        }

        /// <summary>
        /// Menja trenutno aktivni mod za filtering.
        /// </summary>
        public void ChangeTextureGenMode()
        {
            SelectedMode = (TextureGenMode)(((int)m_selectedMode + 1) % Enum.GetNames(typeof(TextureGenMode)).Length);
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
