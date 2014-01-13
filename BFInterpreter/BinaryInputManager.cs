namespace BrainfuckInterpreter
{
    using System;
    using System.IO;

    public class BinaryInputManager : InputManager, IDisposable
    {
        private Stream source;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryInputManager"/> class.
        /// </summary>
        /// <param name="source">Source byte array to read.</param>
        public BinaryInputManager(byte[] source)
        {
            this.source = new MemoryStream(source);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryInputManager"/> class.
        /// </summary>
        /// <param name="source">Source stream to read.</param>
        public BinaryInputManager(Stream source)
        {
            this.source = source;
        }

        /// <summary>
        /// Reads the next integer from the source.
        /// </summary>
        /// <returns>Next read integer, 0 if end of input.</returns>
        public override int Read()
        {
            byte[] tmp = new byte[1];
            if (this.source.Read(tmp, 0, tmp.Length) > 0)
            {
                return tmp[0];
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Disposes all managed resources.
        /// </summary>
        public void Dispose()
        {
            if (this.source != null)
                this.source.Dispose();
        }
    }
}
