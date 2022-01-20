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

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

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

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
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
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_FLAT);
            m_scene.LoadScene();
            m_scene.Initialize();
            cylinder = new Cylinder();
            cylinder.BaseRadius = 0.1;
            cylinder.TopRadius = 0.1;
            cylinder.Height = 3;
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

            //podloga
            gl.Begin(OpenGL.GL_QUADS);
            for (int i = 0; i < pointLinePolygonVertices.Length; i = i + 3)
            {
                gl.Vertex(pointLinePolygonVertices[i], pointLinePolygonVertices[i + 1], pointLinePolygonVertices[i + 2]);
            }
            gl.End();

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



            gl.Translate(-1f, 2f, 0.14f);
            gl.Scale(0.25, 0.25, 0.25);
            m_scene.Draw();

            gl.PopMatrix();

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
