//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: sound_info.proto
namespace TableProto
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"sound_info")]
  public partial class sound_info : global::ProtoBuf.IExtensible
  {
    public sound_info() {}
    
    private int _ID;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"ID", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int ID
    {
      get { return _ID; }
      set { _ID = value; }
    }
    private int _operate_type = (int)0;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"operate_type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)0)]
    public int operate_type
    {
      get { return _operate_type; }
      set { _operate_type = value; }
    }
    private string _path = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"path", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string path
    {
      get { return _path; }
      set { _path = value; }
    }
    private string _name = "";
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }
    private string _describe = "";
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"describe", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string describe
    {
      get { return _describe; }
      set { _describe = value; }
    }
    private int _affiliation_scene = (int)0;
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"affiliation_scene", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)0)]
    public int affiliation_scene
    {
      get { return _affiliation_scene; }
      set { _affiliation_scene = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"sound_info_ARRAY")]
  public partial class sound_info_ARRAY : global::ProtoBuf.IExtensible
  {
    public sound_info_ARRAY() {}
    
    private readonly global::System.Collections.Generic.List<TableProto.sound_info> _items = new global::System.Collections.Generic.List<TableProto.sound_info>();
    [global::ProtoBuf.ProtoMember(1, Name=@"items", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<TableProto.sound_info> items
    {
      get { return _items; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}