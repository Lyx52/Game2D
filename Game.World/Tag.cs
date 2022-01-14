using System.Collections.Generic;
using System.IO;
using System;
using Game.Utils;
namespace Game.World {
 public enum TagType : byte {
        BaseTag,
        StringTag,
        IntTag,
        FloatTag,
        DoubleTag,
        ByteArrayTag,
        CompoundTag,
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
                case TagType.IntTag: {
                    byte[] data = new byte[4];
                    stream.Read(data, 0, 4);

                    return new IntTag(tagName, BitConverter.ToInt32(data));
                }
                case TagType.FloatTag: {
                    byte[] data = new byte[4];
                    stream.Read(data, 0, 4);

                    return new FloatTag(tagName, BitConverter.ToSingle(data));
                }
                case TagType.DoubleTag: {
                    byte[] data = new byte[8];
                    stream.Read(data, 0, 8);

                    return new DoubleTag(tagName,  BitConverter.ToDouble(data));;
                }
                case TagType.CompoundTag: {
                    byte[] data = new byte[4];
                    stream.Read(data, 0, 4);
                    Dictionary<string, Tag> tags = new Dictionary<string, Tag>();
                    for (int i = 0; i < BitConverter.ToInt32(data); i++) {
                        Tag tag = Tag.ReadTag(stream);
                        tags.Add(tag.Name, tag);
                    }
                    
                    return new CompoundTag(tagName, tags);
                }
                case TagType.ByteArrayTag: {
                    byte[] lengthData = new byte[4];
                    stream.Read(lengthData, 0, 4);
                    int length = BitConverter.ToInt32(lengthData);
                    byte[] data = new byte[length];
                    stream.Read(data, 0, BitConverter.ToInt32(lengthData));

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