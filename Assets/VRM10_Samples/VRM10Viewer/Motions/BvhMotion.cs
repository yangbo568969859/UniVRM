using System.Collections.Generic;
using System.IO;
using UniHumanoid;
using UnityEngine;

namespace UniVRM10.VRM10Viewer
{
    public class BvhMotion : IMotion
    {
        UniHumanoid.BvhImporterContext m_context;
        public Transform Root => m_context?.Root.transform;
        public SkinnedMeshRenderer m_boxMan;
        public SkinnedMeshRenderer BoxMan => m_boxMan;
        (INormalizedPoseProvider, ITPoseProvider) m_controlRig;
        (INormalizedPoseProvider, ITPoseProvider) IMotion.ControlRig => m_controlRig;
        public IDictionary<ExpressionKey, Transform> ExpressionMap { get; } = new Dictionary<ExpressionKey, Transform>();
        public BvhMotion(UniHumanoid.BvhImporterContext context)
        {
            m_context = context;
            var provider = new AnimatorPoseProvider(m_context.Root.transform, m_context.Root.GetComponent<Animator>());
            m_controlRig = (provider, provider);

            // create SkinnedMesh for bone visualize
            var animator = m_context.Root.GetComponent<Animator>();
            m_boxMan = SkeletonMeshUtility.CreateRenderer(animator);
            var material = new Material(Shader.Find("Standard"));
            BoxMan.sharedMaterial = material;
            var mesh = BoxMan.sharedMesh;
            mesh.name = "box-man";
        }

        public static BvhMotion LoadBvhFromText(string source, string path = "tmp.bvh")
        {
            var context = new UniHumanoid.BvhImporterContext();
            context.Parse(path, source);
            context.Load();
            return new BvhMotion(context);
        }
        public static BvhMotion LoadBvhFromPath(string path)
        {
            return LoadBvhFromText(File.ReadAllText(path), path);
        }

        public void ShowBoxMan(bool enable)
        {
            m_boxMan.enabled = enable;
        }

        public void Dispose()
        {
            GameObject.Destroy(m_context.Root);
        }
    }
}
