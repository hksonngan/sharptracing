/**
 * Criado por: Thiago Burgo Belo (thiagoburgo@gmail.com)
 * SharpTracing � um projeto feito inicialmente para disciplina
 * Computa��o Gr�fica da UFPE e depois melhorado nos tempos livres
 * sinta-se a vontade para copiar modificar e mandar corre��es e 
 * sugest�es. Mantenha os cr�ditos!
 * **************************************************************
 * SharpTracing is a project originally created to discipline 
 * Computer Graphics of UFPE and was improved in my free time.
 * Feel free to copy, modify and  give fixes 
 * suggestions. Keep the credits!
 */
 using System;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Shaders;

namespace DrawEngine.Renderer.Materials
{
    [Serializable]
    public class PhongMaterial : Material
    {
        private readonly PhongShader shader = new PhongShader();
        public PhongMaterial() : base() {}
        public PhongMaterial(double kdiff, double kspec, double kamb, double refractIndex, double ktrans, double glossy, double shiness,
                             RGBColor color) : base(kdiff, kspec, kamb, refractIndex, ktrans, glossy, shiness, color) {}
        public PhongMaterial(double kdiff, double kspec, double kamb, double refractIndex, double ktrans, double glossy, double shiness,
                             Texture texture)
            : base(kdiff, kspec, kamb, refractIndex, ktrans, glossy, shiness, texture) { }
        public override Shader CreateShader(Scene scene)
        {
            this.shader.Scene = scene;
            return this.shader;
        }
    }
}