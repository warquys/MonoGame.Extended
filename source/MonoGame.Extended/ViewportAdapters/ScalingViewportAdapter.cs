using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Extended.ViewportAdapters
{
    /// <summary>
    /// A viewport adapter that scales a virtual resolution to fit the graphics device viewport.
    /// The virtual dimensions are fixed, and the scale matrix is computed to map virtual
    /// coordinates to the actual viewport size.
    /// </summary>
    public class ScalingViewportAdapter : ViewportAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScalingViewportAdapter"/> class.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device used for rendering.</param>
        /// <param name="virtualWidth">The fixed virtual width in pixels.</param>
        /// <param name="virtualHeight">The fixed virtual height in pixels.</param>
        public ScalingViewportAdapter(GraphicsDevice graphicsDevice, int virtualWidth, int virtualHeight)
            : base(graphicsDevice)
        {
            VirtualWidth = virtualWidth;
            VirtualHeight = virtualHeight;
        }

        /// <inheritdoc/>
        public override int VirtualWidth { get; }

        /// <inheritdoc/>
        public override int VirtualHeight { get; }

        /// <inheritdoc/>
        public override int ViewportWidth => GraphicsDevice.Viewport.Width;

        /// <inheritdoc/>
        public override int ViewportHeight => GraphicsDevice.Viewport.Height;

        /// <inheritdoc/>
        public override Matrix GetScaleMatrix()
        {
            var scaleX = (float)ViewportWidth / VirtualWidth;
            var scaleY = (float)ViewportHeight / VirtualHeight;
            return Matrix.CreateScale(scaleX, scaleY, 1.0f);
        }
    }
}
