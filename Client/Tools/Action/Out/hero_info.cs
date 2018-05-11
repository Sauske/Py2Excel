//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: Proto/hero_info.proto
namespace Proto.hero_info
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HERO_INFO")]
  public partial class HERO_INFO : global::ProtoBuf.IExtensible
  {
    public HERO_INFO() {}
    
    private int _ID;
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"ID", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int ID
    {
      get { return _ID; }
      set { _ID = value; }
    }
    private string _hero_res = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"hero_res", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string hero_res
    {
      get { return _hero_res; }
      set { _hero_res = value; }
    }
    private string _name = "";
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }
    private int _sex = (int)0;
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"sex", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)0)]
    public int sex
    {
      get { return _sex; }
      set { _sex = value; }
    }
    private int _type = (int)0;
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)0)]
    public int type
    {
      get { return _type; }
      set { _type = value; }
    }
    private int _camp = (int)0;
    [global::ProtoBuf.ProtoMember(11, IsRequired = false, Name=@"camp", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)0)]
    public int camp
    {
      get { return _camp; }
      set { _camp = value; }
    }
    private int _quality = (int)0;
    [global::ProtoBuf.ProtoMember(13, IsRequired = false, Name=@"quality", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)0)]
    public int quality
    {
      get { return _quality; }
      set { _quality = value; }
    }
    private int _level_max = (int)0;
    [global::ProtoBuf.ProtoMember(15, IsRequired = false, Name=@"level_max", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue((int)0)]
    public int level_max
    {
      get { return _level_max; }
      set { _level_max = value; }
    }
    private string _hero_icon = "";
    [global::ProtoBuf.ProtoMember(17, IsRequired = false, Name=@"hero_icon", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string hero_icon
    {
      get { return _hero_icon; }
      set { _hero_icon = value; }
    }
    private string _attack_skill = "";
    [global::ProtoBuf.ProtoMember(19, IsRequired = false, Name=@"attack_skill", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string attack_skill
    {
      get { return _attack_skill; }
      set { _attack_skill = value; }
    }
    private string _passive_skill = "";
    [global::ProtoBuf.ProtoMember(21, IsRequired = false, Name=@"passive_skill", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string passive_skill
    {
      get { return _passive_skill; }
      set { _passive_skill = value; }
    }
    private string _skill = "";
    [global::ProtoBuf.ProtoMember(23, IsRequired = false, Name=@"skill", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string skill
    {
      get { return _skill; }
      set { _skill = value; }
    }
    private string _ultimate_skill = "";
    [global::ProtoBuf.ProtoMember(25, IsRequired = false, Name=@"ultimate_skill", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string ultimate_skill
    {
      get { return _ultimate_skill; }
      set { _ultimate_skill = value; }
    }
    private string _description = "";
    [global::ProtoBuf.ProtoMember(27, IsRequired = false, Name=@"description", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string description
    {
      get { return _description; }
      set { _description = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"HERO_INFO_ARRAY")]
  public partial class HERO_INFO_ARRAY : global::ProtoBuf.IExtensible
  {
    public HERO_INFO_ARRAY() {}
    
    private readonly global::System.Collections.Generic.List<HERO_INFO> _hero_info_items = new global::System.Collections.Generic.List<HERO_INFO>();
    [global::ProtoBuf.ProtoMember(1, Name=@"hero_info_items", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<HERO_INFO> hero_info_items
    {
      get { return _hero_info_items; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}