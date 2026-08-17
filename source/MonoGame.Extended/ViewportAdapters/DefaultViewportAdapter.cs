using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// ReSharper disable once CheckNamespace

namespace MonoGame.Extended.ViewportAdapters
{
    /// <summary>
    /// A viewport adapter that uses the graphics device's current viewport as both
    /// the virtual and screen dimensions. The scale matrix is identity,
    /// meaning no scaling is applied between virtual and screen coordinates.
    /// </summary>
    public class DefaultViewportAdapter : ViewportAdapter
    {
        private readonly GraphicsDevice _graphicsDevice;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultViewportAdapter"/> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device used for rendering.</param>
        public DefaultViewportAdapter(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        /// <inheritdoc/>
        public override int VirtualWidth => _graphicsDevice.Viewport.Width;

        /// <inheritdoc/>
        public override int VirtualHeight => _graphicsDevice.Viewport.Height;

        /// <inheritdoc/>
        public override int ViewportWidth => _graphicsDevice.Viewport.Width;

        /// <inheritdoc/>
        public override int ViewportHeight => _graphicsDevice.Viewport.Height;

        /// <inheritdoc/>
        public override Matrix GetScaleMatrix()
        {
            return Matrix.Identity;
        }
    }
}
