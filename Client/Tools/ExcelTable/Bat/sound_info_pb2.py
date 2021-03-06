# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: sound_info.proto

from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import descriptor_pb2
# @@protoc_insertion_point(imports)




DESCRIPTOR = _descriptor.FileDescriptor(
  name='sound_info.proto',
  package='TableProto',
  serialized_pb='\n\x10sound_info.proto\x12\nTableProto\"\x86\x01\n\nsound_info\x12\r\n\x02ID\x18\x01 \x02(\x05:\x01\x30\x12\x17\n\x0coperate_type\x18\x02 \x01(\x05:\x01\x30\x12\x0e\n\x04path\x18\x03 \x01(\t:\x00\x12\x0e\n\x04name\x18\x04 \x01(\t:\x00\x12\x12\n\x08\x64\x65scribe\x18\x05 \x01(\t:\x00\x12\x1c\n\x11\x61\x66\x66iliation_scene\x18\x06 \x01(\x05:\x01\x30\"9\n\x10sound_info_ARRAY\x12%\n\x05items\x18\x01 \x03(\x0b\x32\x16.TableProto.sound_info')




_SOUND_INFO = _descriptor.Descriptor(
  name='sound_info',
  full_name='TableProto.sound_info',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='ID', full_name='TableProto.sound_info.ID', index=0,
      number=1, type=5, cpp_type=1, label=2,
      has_default_value=True, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None),
    _descriptor.FieldDescriptor(
      name='operate_type', full_name='TableProto.sound_info.operate_type', index=1,
      number=2, type=5, cpp_type=1, label=1,
      has_default_value=True, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None),
    _descriptor.FieldDescriptor(
      name='path', full_name='TableProto.sound_info.path', index=2,
      number=3, type=9, cpp_type=9, label=1,
      has_default_value=True, default_value=unicode("", "utf-8"),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None),
    _descriptor.FieldDescriptor(
      name='name', full_name='TableProto.sound_info.name', index=3,
      number=4, type=9, cpp_type=9, label=1,
      has_default_value=True, default_value=unicode("", "utf-8"),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None),
    _descriptor.FieldDescriptor(
      name='describe', full_name='TableProto.sound_info.describe', index=4,
      number=5, type=9, cpp_type=9, label=1,
      has_default_value=True, default_value=unicode("", "utf-8"),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None),
    _descriptor.FieldDescriptor(
      name='affiliation_scene', full_name='TableProto.sound_info.affiliation_scene', index=5,
      number=6, type=5, cpp_type=1, label=1,
      has_default_value=True, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  options=None,
  is_extendable=False,
  extension_ranges=[],
  serialized_start=33,
  serialized_end=167,
)


_SOUND_INFO_ARRAY = _descriptor.Descriptor(
  name='sound_info_ARRAY',
  full_name='TableProto.sound_info_ARRAY',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='items', full_name='TableProto.sound_info_ARRAY.items', index=0,
      number=1, type=11, cpp_type=10, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      options=None),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  options=None,
  is_extendable=False,
  extension_ranges=[],
  serialized_start=169,
  serialized_end=226,
)

_SOUND_INFO_ARRAY.fields_by_name['items'].message_type = _SOUND_INFO
DESCRIPTOR.message_types_by_name['sound_info'] = _SOUND_INFO
DESCRIPTOR.message_types_by_name['sound_info_ARRAY'] = _SOUND_INFO_ARRAY

class sound_info(_message.Message):
  __metaclass__ = _reflection.GeneratedProtocolMessageType
  DESCRIPTOR = _SOUND_INFO

  # @@protoc_insertion_point(class_scope:TableProto.sound_info)

class sound_info_ARRAY(_message.Message):
  __metaclass__ = _reflection.GeneratedProtocolMessageType
  DESCRIPTOR = _SOUND_INFO_ARRAY

  # @@protoc_insertion_point(class_scope:TableProto.sound_info_ARRAY)


# @@protoc_insertion_point(module_scope)
