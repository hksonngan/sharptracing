using System;
using System.Drawing;
using DrawEngine.Renderer.Algebra;
using DrawEngine.Renderer.BasicStructures;
using DrawEngine.Renderer.Materials;
using DrawEngine.Renderer.Shaders;

namespace DrawEngine.Renderer.Tracers
{
    public sealed class DistributedRayTracer : RayCasting
    {
        public DistributedRayTracer(Scene scene) : base(scene) {}
        public DistributedRayTracer() : base() {}
        public override /*Bitmap*/ void Render(Graphics g)
        {
            float resX = this.scene.DefaultCamera.ResX; //g.VisibleClipBounds.Width; 
            float resY = this.scene.DefaultCamera.ResY; //g.VisibleClipBounds.Height;
            float x, y;
            int iterations = 0;
            RGBColor rgb = new RGBColor();
            int pCol = 0, pRow = 0, pIteration = 1, pMax = 2;
            SolidBrush brush = new SolidBrush(Color.Black);
            float resTotal = resX * resY;
            while(iterations < resTotal){
                //Render Pixels Out of Order With Increasing Resolution: 2x2, 4x4, 16x16... 512x512
                if(pCol >= pMax){
                    pRow++;
                    pCol = 0;
                    if(pRow >= pMax){
                        pIteration++;
                        pRow = 0;
                        pMax = (int)Math.Pow(2, pIteration);
                    }
                }
                bool pNeedsDrawing = (pIteration == 1 || (pRow % 2 != 0) || (!(pRow % 2 != 0) && (pCol % 2 != 0)));
                x = pCol * (resX / pMax);
                y = pRow * (resY / pMax);
                pCol++;
                if(pNeedsDrawing){
                    iterations++;
                    Ray ray = this.scene.DefaultCamera.CreateRayFromScreen(x, y);
                    ray.PrevRefractIndex = this.scene.RefractIndex;
                    rgb = this.Trace(ray, 0);
                    brush.Color = rgb.ToColor();
                    g.FillRectangle(brush, x, y, (resX / pMax), (resY / pMax));
                }
            }
        }
        public RGBColor Trace(Ray ray, int depth)
        {
            Intersection intersection;
            if(this.scene.FindIntersection(ray, out intersection)){
                Material material = intersection.HitPrimitive.Material;
                //pode passar o shade pra primitiva
                if(material is CookTorranceMaterial){
                    this.scene.Shader = new CookTorranceShader(this.scene);
                } else{
                    this.scene.Shader = new PhongShader(this.scene);
                    //this.shader = new PerlinNoiseShader(this.scene);
                }
                RGBColor color = this.scene.Shader.Shade(ray, intersection);
                Ray rRay = new Ray();
                if(depth < 5){
                    if(material.IsReflective){
                        Vector3D reflected = Reflected(intersection.Normal, ray.Direction);
                        rRay.Origin = intersection.HitPoint;
                        rRay.Direction = reflected;
                        //color += Trace(rRay, depth + 1) * material.KSpec;
                        RGBColor[] colors = new RGBColor[30];
                        //colors[0] = color;
                        for(int i = 0; i < 30; i++){
                            rRay.Direction = BlurryReflected(reflected);
                            colors[i] = this.Trace(rRay, 5) * material.KSpec;
                        }
                        color += AverageColors(colors);
                    }
                    if(material.IsTransparent){
                        Vector3D T;
                        //float eta = intersection.HitFromInSide
                        //                ? material.RefractIndex * 1 / this.scene.RefractIndex
                        //                : this.scene.RefractIndex * 1 / material.RefractIndex;
                        float eta = this.scene.RefractIndex * 1 / material.RefractIndex;
                        //(ray.PrevRefractIndex == material.RefractIndex)
                        //                ? material.RefractIndex * 1 / this.scene.RefractIndex
                        //                : this.scene.RefractIndex * 1 / material.RefractIndex;
                        if(Refracted(intersection.Normal, ray.Direction, out T, eta)){
                            rRay.Origin = intersection.HitPoint;
                            rRay.Direction = T;
                            rRay.PrevRefractIndex = material.RefractIndex;
                            color += this.Trace(rRay, depth + 1) * material.KTrans;
                        }
                    }
                }
                return color;
            }
            return RGBColor.Black;
        }
        private static Vector3D BlurryReflected(Vector3D reflected)
        {
            Random rdn = new Random();
            float x = (-1 + 2 * (float)rdn.NextDouble());
            float y = (-1 + 2 * (float)rdn.NextDouble());
            float z = (-1 + 2 * (float)rdn.NextDouble());
            Vector3D u = new Vector3D(x, y, z);
            Vector3D.Orthogonalize(ref u, reflected);
            x = (-1 + 2 * (float)rdn.NextDouble());
            y = (-1 + 2 * (float)rdn.NextDouble());
            z = (-1 + 2 * (float)rdn.NextDouble());
            Vector3D v = new Vector3D(x, y, z);
            Vector3D.Orthogonalize(ref v, u);
            const float blurSize = .05f;
            Vector3D blurred = reflected + ((u * blurSize) + (v * blurSize));
            blurred.Normalize();
            return blurred;
        }
    }
}