// <copyright file="InputManager.cs" company="nXu.hu">
//     Copyright nXu. Licensed under the MIT License.
// </copyright>
// <author>nXu</author>

namespace BrainfuckInterpreter
{
    using System;

    /// <summary>
    /// An abstract class defining methods for brainfuck input.
    /// </summary>
    public abstract class InputManager
    {
        /// <summary>
        /// Reads the next integer from the source.
        /// </summary>
        /// <returns>Next read integer, 0 if end of input.</returns>
        public abstract int Read();

        /// <summary>
        /// Gets the Read() method as a function object.
        /// </summary>
        /// <returns>A valid brainfuck input function.</returns>
        public virtual Func<int> GetInputFunction()
        {
            return new Func<int>(() => this.Read());
        }
    }
}
