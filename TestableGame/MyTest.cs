using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Scurvy.Test;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TestableGame
{
    [TestClass]
    public class MyTest
    {
        private ContentManager content;

        public MyTest()
        {
        }

        [TestSetup]
        public void Setup(TestContext context)
        {
            this.content = new ContentManager(context.Services, "Content");
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.content.Unload();
            this.content.Dispose();
        }

        /// <summary>
        /// This test makes sure that the scurvy_logo_big content loads successfully.
        /// </summary>
        [TestMethod]
        public void ContentLoadTest()
        {
            Texture2D texture = this.content.Load<Texture2D>("scurvy_logo_bigX");
            Assert.IsTrue(texture != null);
        }

        /// <summary>
        /// This test renders a texture and waits for manual approval that the test is finished.
        /// </summary>
        [TestMethod]
        public void ManualContentVerificationTest(TestContext context)
        {
            context.ExitCriteria = new ContentVerificationExitCriteria(this.content, context);
        }

        private class ContentVerificationExitCriteria : ExitCriteria
        {
            private SpriteBatch batch;
            private Texture2D texture;
            private SpriteFont font;
            private float xPosition;

            public ContentVerificationExitCriteria(ContentManager content, TestContext context)
                : base(context)
            {
                IGraphicsDeviceService graphics = (IGraphicsDeviceService)this.Context.Services.GetService(typeof(IGraphicsDeviceService));
                this.batch = new SpriteBatch(graphics.GraphicsDevice);
                this.texture = content.Load<Texture2D>("scurvy_logo_big");
                this.font = content.Load<SpriteFont>("font");
            }

            public override void Update(TimeSpan elapsedTime)
            {
                KeyboardState keyState = Keyboard.GetState();
                GamePadState padState = GamePad.GetState(PlayerIndex.One);

                xPosition += elapsedTime.Milliseconds * .01f;

                if (keyState.IsKeyDown(Keys.Enter) || keyState.IsKeyDown(Keys.Space) || padState.IsButtonDown(Buttons.A))
                {
                    this.IsFinished = true;
                }
            }

            public override void Draw()
            {
                batch.Begin();
                batch.DrawString(font, "Press Enter when finished verifying this test", new Vector2(50, 50), Color.White);
                batch.Draw(this.texture, new Vector2((float)Math.Sin(xPosition) * 100, 100), Color.White);
                batch.End();
            }
        }
    }
}
