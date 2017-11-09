﻿using System;
using System.Collections.Generic;

namespace Veldrid.OpenGL.ManagedEntryList
{
    internal class OpenGLManagedCommandEntryList : OpenGLCommandEntryList
    {
        private readonly List<OpenGLCommandEntry> _commands = new List<OpenGLCommandEntry>();
        private readonly StagingMemoryPool _memoryPool = new StagingMemoryPool();

        public IReadOnlyList<OpenGLCommandEntry> Commands => _commands;

        public void Reset()
        {
            _commands.Clear();
        }

        public void Begin()
        {
            _commands.Add(new BeginEntry());
        }

        public void ClearColorTarget(uint index, RgbaFloat clearColor)
        {
            _commands.Add(new ClearColorTargetEntry(index, clearColor));
        }

        public void ClearDepthTarget(float depth)
        {
            _commands.Add(new ClearDepthTargetEntry(depth));
        }

        public void Draw(uint indexCount, uint instanceCount, uint indexStart, int vertexOffset, uint instanceStart)
        {
            _commands.Add(new DrawEntry(indexCount, instanceCount, indexStart, vertexOffset, instanceStart));
        }

        public void End()
        {
            _commands.Add(new EndEntry());
        }

        public void SetFramebuffer(Framebuffer fb)
        {
            _commands.Add(new SetFramebufferEntry(fb));
        }

        public void SetIndexBuffer(IndexBuffer ib)
        {
            _commands.Add(new SetIndexBufferEntry(ib));
        }

        public void SetPipeline(Pipeline pipeline)
        {
            _commands.Add(new SetPipelineEntry(pipeline));
        }

        public void SetResourceSet(uint slot, ResourceSet rs)
        {
            _commands.Add(new SetResourceSetEntry(slot, rs));
        }

        public void SetScissorRect(uint index, uint x, uint y, uint width, uint height)
        {
            _commands.Add(new SetScissorRectEntry(index, x, y, width, height));
        }

        public void SetVertexBuffer(uint index, VertexBuffer vb)
        {
            _commands.Add(new SetVertexBufferEntry(index, vb));
        }

        public void SetViewport(uint index, ref Viewport viewport)
        {
            _commands.Add(new SetViewportEntry(index, ref viewport));
        }

        public void UpdateBuffer(Buffer buffer, uint bufferOffsetInBytes, IntPtr source, uint sizeInBytes)
        {
            StagingBlock stagingBlock = _memoryPool.Stage(source, sizeInBytes);
            _commands.Add(new UpdateBufferEntry(buffer, bufferOffsetInBytes, stagingBlock));
        }

        public void UpdateTexture(
            Texture texture,
            IntPtr source,
            uint sizeInBytes,
            uint x,
            uint y,
            uint z,
            uint width,
            uint height,
            uint depth,
            uint mipLevel,
            uint arrayLayer)
        {
            StagingBlock stagingBlock = _memoryPool.Stage(source, sizeInBytes);
            _commands.Add(new UpdateTextureEntry(texture, stagingBlock, x, y, z, width, height, depth, mipLevel, arrayLayer));
        }

        public void UpdateTextureCube(
            Texture textureCube,
            IntPtr source,
            uint sizeInBytes,
            CubeFace face,
            uint x,
            uint y,
            uint width,
            uint height,
            uint mipLevel,
            uint arrayLayer)
        {
            StagingBlock stagingBlock = _memoryPool.Stage(source, sizeInBytes);
            _commands.Add(
                new UpdateTextureCubeEntry(textureCube, stagingBlock, face, x, y, width, height, mipLevel, arrayLayer));
        }

        public void ResolveTexture(Texture source, Texture destination)
        {
            _commands.Add(new ResolveTextureEntry(source, destination));

        }

        public void ExecuteAll(OpenGLCommandExecutor executor)
        {
            foreach (OpenGLCommandEntry entry in _commands)
            {
                switch (entry)
                {
                    case BeginEntry be:
                        executor.Begin();
                        break;
                    case ClearColorTargetEntry ccte:
                        executor.ClearColorTarget(ccte.Index, ccte.ClearColor);
                        break;
                    case ClearDepthTargetEntry cdte:
                        executor.ClearDepthTarget(cdte.Depth);
                        break;
                    case DrawEntry de:
                        executor.Draw(de.IndexCount, de.InstanceCount, de.IndexStart, de.VertexOffset, de.InstanceCount);
                        break;
                    case EndEntry ee:
                        executor.End();
                        break;
                    case SetFramebufferEntry sfbe:
                        executor.SetFramebuffer(sfbe.Framebuffer);
                        break;
                    case SetIndexBufferEntry sibe:
                        executor.SetIndexBuffer(sibe.IndexBuffer);
                        break;
                    case SetPipelineEntry spe:
                        executor.SetPipeline(spe.Pipeline);
                        break;
                    case SetResourceSetEntry srse:
                        executor.SetResourceSet(srse.Slot, srse.ResourceSet);
                        break;
                    case SetScissorRectEntry ssre:
                        executor.SetScissorRect(ssre.Index, ssre.X, ssre.Y, ssre.Width, ssre.Height);
                        break;
                    case SetVertexBufferEntry svbe:
                        executor.SetVertexBuffer(svbe.Index, svbe.VertexBuffer);
                        break;
                    case SetViewportEntry sve:
                        executor.SetViewport(sve.Index, ref sve.Viewport);
                        break;
                    case UpdateBufferEntry ube:
                        executor.UpdateBuffer(ube.Buffer, ube.BufferOffsetInBytes, ube.StagingBlock);
                        break;
                    case UpdateTextureEntry ute:
                        executor.UpdateTexture(
                            ute.Texture,
                            ute.StagingBlock,
                            ute.X,
                            ute.Y,
                            ute.Z,
                            ute.Width,
                            ute.Height,
                            ute.Depth,
                            ute.MipLevel,
                            ute.ArrayLayer);
                        break;
                    case UpdateTextureCubeEntry utce:
                        executor.UpdateTextureCube(
                            utce.TextureCube,
                            utce.StagingBlock,
                            utce.Face,
                            utce.X,
                            utce.Y,
                            utce.Width,
                            utce.Height,
                            utce.MipLevel,
                            utce.ArrayLayer);
                        break;
                    case ResolveTextureEntry rte:
                        executor.ResolveTexture(rte.Source, rte.Destination);
                        break;
                    default:
                        throw new InvalidOperationException("Command type not handled: " + executor.GetType().Name);
                }
            }
        }
    }
}
