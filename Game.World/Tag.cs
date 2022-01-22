using System.Collections.Generic;
using System.IO;
using System;
using OpenTK.Mathematics;
using Game.Utils;

namespace Game.World {
 public enum TagType : byte {
        BaseTag,
        StringTag,
        IntTag,
        UIntTag,
        LongTag,
        ULongTag,
        FloatTag,
        DoubleTag,
        ByteArrayTag,
        CompoundTag,
        Vector2Tag,
        Vector2iTag,
        EndTag
    }
    public abstract class Tag {
        public string Name { get; set; }
        public TagType Type { get; set; }
        public Tag(string name, TagType type) {
            this.Name = name;
            this.Type = type;
        }
        public static string TagsAsString(List<Tag> tags) {
            string output = "";
            foreach(Tag tag in tags) {
                output += $"{tag.ToString()}\n";
            }
            return output;
        }
        public abstract void WriteTag(FileStream stream);
        public static TagType ReadTagType(FileStream stream) {
            return (TagType)stream.ReadByte();
        }
        public static TagType PeekNextTag(FileStream stream) {
            // Cannot read next tag since file is empty
            if (stream.Length <= 0)
                return TagType.EndTag;

            TagType type = ReadTagType(stream);
            stream.Position--;
            return type;
        }
        public static void WriteTags(FileStream stream, List<Tag> tags) {
            // Add end tag if it doesnt exist
            if (!tags.Exists(tag => tag.Type == TagType.EndTag))
                tags.Add(new EndTag());

            foreach (Tag tag in tags) {
                tag.WriteTag(stream);
            }
        } 
        public static List<Tag> ReadTags(FileStream stream) {
            List<Tag> tags = new List<Tag>();
            while (Tag.PeekNextTag(stream) != TagType.EndTag) {
                tags.Add(Tag.ReadTag(stream));
            }
            return tags;
        }
        public static Tag ReadTag(FileStream stream) {
            TagType type = ReadTagType(stream);
            string tagName = IOUtils.ReadString(stream);
            switch(type) {
                case TagType.StringTag: 
                    return new StringTag(tagName, IOUtils.ReadString(stream));
                case TagType.IntTag:    return new IntTag(tagName, IOUtils.ReadInt32(stream));
                case TagType.UIntTag:    return new UIntTag(tagName, IOUtils.ReadUInt32(stream));
                case TagType.LongTag:    return new LongTag(tagName, IOUtils.ReadInt64(stream));
                case TagType.ULongTag:    return new ULongTag(tagName, IOUtils.ReadUInt64(stream));
                case TagType.FloatTag:  return new FloatTag(tagName, IOUtils.ReadFloat(stream));
                case TagType.DoubleTag: return new DoubleTag(tagName,  IOUtils.ReadDouble(stream));;

                // Vector2/Vector2i first float read is X second is Y
                case TagType.Vector2Tag: return new Vector2Tag(tagName, new Vector2(IOUtils.ReadFloat(stream), IOUtils.ReadFloat(stream)));
                case TagType.Vector2iTag: return new Vector2iTag(tagName, new Vector2i(IOUtils.ReadInt32(stream), IOUtils.ReadInt32(stream)));             
                case TagType.CompoundTag: {
                    Dictionary<string, Tag> tags = new Dictionary<string, Tag>();
                    int tagCount = IOUtils.ReadInt32(stream);
                    for (int i = 0; i < tagCount; i++) {
                        Tag tag = Tag.ReadTag(stream);
                        tags.Add(tag.Name, tag);
                    }
                    
                    return new CompoundTag(tagName, tags);
                }
                case TagType.ByteArrayTag: {
                    int length = IOUtils.ReadInt32(stream);
                    byte[] data = new byte[length];
                    stream.Read(data, 0, length);
                    return new ByteArrayTag(tagName, data);
                }
                case TagType.BaseTag:
                case TagType.EndTag:
                default:
                    throw new InvalidDataException($"Invalid tag type {type}!");
            }
        }
        public override string ToString()
        {
            return $"{this.Type}[{this.Name}]";
        }
    }
    public class EndTag : Tag {
        public EndTag() : base("END_TAG", TagType.EndTag) {}
        public override void WriteTag(FileStream stream)
        {
            // Write tag identificator
            stream.WriteByte((byte)this.Type);
            
            // Write tag name
            stream.Write(StringUtils.StringAsBytes(this.Name));
        }
    }
    public class IntTag : Tag {
        public int Value;
        public IntTag(string name) : this(name, 0) {}
        public IntTag(string name, int value) : base(name, TagType.IntTag) {
            this.Value = value;
        }
        public override void WriteTag(FileStream stream)
        {
            // Write tag identificator
            stream.WriteByte((byte)this.Type);
            
            // Write tag name
            stream.Write(StringUtils.StringAsBytes(this.Name));

            // Write data
            stream.Write(BitConverter.GetBytes(this.Value));
        }
        public override string ToString()
        {
            return $"{base.ToString()} - {this.Value}";
        }
    }
    public class UIntTag : Tag {
        public uint Value;
        public UIntTag(string name) : this(name, 0) {}
        public UIntTag(string name, uint value) : base(name, TagType.UIntTag) {
            this.Value = value;
        }
        public override void WriteTag(FileStream stream)
        {
            // Write tag identificator
            stream.WriteByte((byte)this.Type);
            
            // Write tag name
            stream.Write(StringUtils.StringAsBytes(this.Name));

            // Write data
            stream.Write(BitConverter.GetBytes(this.Value));
        }
        public override string ToString()
        {
            return $"{base.ToString()} - {this.Value}";
        }
    }
    public class LongTag : Tag {
        public long Value;
        public LongTag(string name) : this(name, 0L) {}
        public LongTag(string name, long value) : base(name, TagType.LongTag) {
            this.Value = value;
        }
        public override void WriteTag(FileStream stream)
        {
            // Write tag identificator
            stream.WriteByte((byte)this.Type);
            
            // Write tag name
            stream.Write(StringUtils.StringAsBytes(this.Name));

            // Write data
            stream.Write(BitConverter.GetBytes(this.Value));
        }
        public override string ToString()
        {
            return $"{base.ToString()} - {this.Value}";
        }
    }
    public class ULongTag : Tag {
        public ulong Value;
        public ULongTag(string name) : this(name, 0L) {}
        public ULongTag(string name, ulong value) : base(name, TagType.ULongTag) {
            this.Value = value;
        }
        public override void WriteTag(FileStream stream)
        {
            // Write tag identificator
            stream.WriteByte((byte)this.Type);
            
            // Write tag name
            stream.Write(StringUtils.StringAsBytes(this.Name));

            // Write data
            stream.Write(BitConverter.GetBytes(this.Value));
        }
        public override string ToString()
        {
            return $"{base.ToString()} - {this.Value}";
        }
    }
    public class DoubleTag : Tag {
        public double Value;
        public DoubleTag(string name) : this(name, 0) {}
        public DoubleTag(string name, double value) : base(name, TagType.DoubleTag) {
            this.Value = value;
        }
        public override void WriteTag(FileStream stream)
        {
            // Write tag identificator
            stream.WriteByte((byte)this.Type);
            
            // Write tag name
            stream.Write(StringUtils.StringAsBytes(this.Name));

            // Write data
            stream.Write(BitConverter.GetBytes(this.Value));
        }
        public override string ToString()
        {
            return $"{base.ToString()} - {this.Value}";
        }
    }
    public class FloatTag : Tag {
        public float Value;
        public FloatTag(string name) : this(name, 0) {}
        public FloatTag(string name, float value) : base(name, TagType.FloatTag) {
            this.Value = value;
        }
        public override void WriteTag(FileStream stream)
        {
            // Write tag identificator
            stream.WriteByte((byte)this.Type);
            
            // Write tag name
            stream.Write(StringUtils.StringAsBytes(this.Name));

            // Write data
            stream.Write(BitConverter.GetBytes(this.Value));
        }
        public override string ToString()
        {
            return $"{base.ToString()} - {this.Value}";
        }
    }
    public class StringTag : Tag {
        public string Value;
        public StringTag(string name) : this(name, "") {}
        public StringTag(string name, string value) : base(name, TagType.StringTag) {
            this.Value = value;
        }
        public override void WriteTag(FileStream stream)
        {
            // Write tag identificator
            stream.WriteByte((byte)this.Type);
            
            // Write tag name
            stream.Write(StringUtils.StringAsBytes(this.Name));

            // Write data
            stream.Write(StringUtils.StringAsBytes(this.Value));
        }
        public override string ToString()
        {
            return $"{base.ToString()} - {this.Value}";
        }
    }
    public class ByteArrayTag : Tag {
        public byte[] Value;
        public ByteArrayTag(string name) : this(name, default(byte[])) {}
        public ByteArrayTag(string name, in byte[] value) : base(name, TagType.ByteArrayTag) {
            this.Value = value;
        }
        public override void WriteTag(FileStream stream)
        {
            // Write tag identificator
            stream.WriteByte((byte)this.Type);
            
            // Write tag name
            stream.Write(StringUtils.StringAsBytes(this.Name));

            // Write length
            stream.Write(BitConverter.GetBytes(this.Value.Length));

            // Write data
            stream.Write(this.Value);
        }
    }
    public class Vector2Tag : Tag {
        public Vector2 Value;
        public Vector2Tag(string name) : this(name, Vector2.Zero) {}
        public Vector2Tag(string name, Vector2 value) : base(name, TagType.Vector2Tag) {
            this.Value = value;
        }
        public override void WriteTag(FileStream stream)
        {
            // Write tag identificator
            stream.WriteByte((byte)this.Type);
            
            // Write tag name
            stream.Write(StringUtils.StringAsBytes(this.Name));

            // Write data X, Y
            stream.Write(BitConverter.GetBytes(this.Value.X));
            stream.Write(BitConverter.GetBytes(this.Value.Y));
        }
        public override string ToString()
        {
            return $"{base.ToString()} - {this.Value}";
        }
    }
    public class Vector2iTag : Tag {
        public Vector2i Value;
        public Vector2iTag(string name) : this(name, Vector2i.Zero) {}
        public Vector2iTag(string name, Vector2i value) : base(name, TagType.Vector2iTag) {
            this.Value = value;
        }
        public override void WriteTag(FileStream stream)
        {
            // Write tag identificator
            stream.WriteByte((byte)this.Type);
            
            // Write tag name
            stream.Write(StringUtils.StringAsBytes(this.Name));

            // Write data X, Y
            stream.Write(BitConverter.GetBytes(this.Value.X));
            stream.Write(BitConverter.GetBytes(this.Value.Y));
        }
        public override string ToString()
        {
            return $"{base.ToString()} - {this.Value}";
        }
    }
    public class CompoundTag : Tag {
        public Dictionary<string, Tag> Tags { get; set; }
        public CompoundTag(string name) : this(name, new Dictionary<string, Tag>()) {}
        public CompoundTag(string name, List<Tag> tags) : base(name, TagType.CompoundTag) {
            this.Tags = new Dictionary<string, Tag>();
            foreach (Tag tag in tags)
                this.Tags.Add(tag.Name, tag);
        }
        public CompoundTag(string name, Dictionary<string, Tag> tags) : base(name, TagType.CompoundTag) {
            this.Tags = tags;
        }
        public bool Contains(string key) {
            return this.Tags.ContainsKey(key);
        }
        public void AddTag(Tag tag) {
            if (!this.Tags.TryAdd(tag.Name, tag)) {
                GameHandler.Logger.Error($"Tag with the name {tag.Name} already exists!");
            }
        }
        public CompoundTag GetCompoundTag(string key) {
            return (CompoundTag)this.GetTag(key);
        }
        public DoubleTag GetDoubleTag(string key) {
            return (DoubleTag)this.GetTag(key);
        }
        public FloatTag GetFloatTag(string key) {
            return (FloatTag)this.GetTag(key);
        }
        public IntTag GetIntTag(string key) {
            return (IntTag)this.GetTag(key);
        }
        public StringTag GetStringTag(string key) {
            return (StringTag)this.GetTag(key);
        }
        public ByteArrayTag GetByteArrayTag(string key) {
            return (ByteArrayTag)this.GetTag(key);
        }
        public Vector2Tag GetVector2Tag(string key) {
            return (Vector2Tag)this.GetTag(key);
        }
        public Vector2iTag GetVector2iTag(string key) {
            return (Vector2iTag)this.GetTag(key);
        }
        private Tag GetTag(string key) {
            if (this.Tags.TryGetValue(key, out Tag tag)) {
                return tag;
            } else {
                GameHandler.Logger.Critical($"Compound tag {this.Name} does not contain {key}!");
                return null;
            }
        }
        public override void WriteTag(FileStream stream)
        {
            // Write tag identificator
            stream.WriteByte((byte)this.Type);
            
            // Write tag name
            stream.Write(StringUtils.StringAsBytes(this.Name));

            // Write number of tags it contains
            stream.Write(BitConverter.GetBytes(this.Tags.Count));

            // Write each of the tags individually
            foreach (Tag tag in this.Tags.Values) {
                tag.WriteTag(stream);
            }
        }
        public override string ToString()
        {
            string output = $"{base.ToString()} - (";
            foreach (string key in this.Tags.Keys) {
                output += $"{this.Tags[key].ToString()}";
            }
            return output + ")";
        }
    }
}