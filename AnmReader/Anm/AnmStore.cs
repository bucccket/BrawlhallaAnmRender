using BrawlhallaANMReader.ANM.Utils;

namespace BrawlhallaANMReader.ANM.Anm
{
    ///<summary>Class <c>AnmStore</c> is a collection of animations.</summary>
    public class AnmStore
    {
        ///<value>Name of the store.</value>
        public string Name { get; set; } = default!;

        ///<value>Index of the store.</value>
        public string Index { get; set; } = default!;

        ///<value>Name of the file.</value>
        public string FileName { get; set; } = default!;

        ///<value>Number of animations in the store.</value>
        public uint AnimationCount { get; set; } = default!;

        ///<value>Animations in the store.</value>
        public List<AnmAnimation> Animations { get; set; } = new();

        ///<summary>Initialises a new animation store.</summary>
        public AnmStore() { }

        ///<summary>Parses an animation store from a <c>ByteStream</c>.</summary>
        ///<param name="buffer">The <c>ByteStream</c> to parse from.</param>
        ///<exception cref="AnmParsingException">Thrown when the <c>ByteStream</c> is too short.</exception>
        public void Parse(ByteStream buffer)
        {
            try
            {
                Name = buffer.ReadString();
                Index = buffer.ReadString();
                FileName = buffer.ReadString();
                AnimationCount = buffer.ReadUInt();
            }
            catch (IndexOutOfRangeException)
            {
                Logger.Error("AnmStore: Animation store parsing error.  Buffer reached end unexpectedly.");
                throw new AnmParsingException("Animation store parsing error.  Buffer reached end unexpectedly.");
            }
            for (int i = 0; i < AnimationCount; i++)
            {
                AnmAnimation animation = new();
                animation.Parse(buffer);
                Animations.Add(animation);
            }
        }

        ///<summary>Gets an animation by name.</summary>
        ///<param name="index">The name of the animation to get.</param>
        ///<returns>The animation with the given name.</returns>
        ///<exception cref="AnmParsingException">Thrown when the animation with the given name does not exist.</exception>
        public AnmAnimation GetAnimationByName(string index)
        {
            foreach (AnmAnimation animation in Animations) if (animation.Name.Contains(index)) return animation;
            Logger.Error($"AnmStore: Animation with name {index} does not exist.");
            throw new AnmParsingException($"Animation with name {index} does not exist.");
        }
    }
}
