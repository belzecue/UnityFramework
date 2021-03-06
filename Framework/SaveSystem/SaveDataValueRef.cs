using System;

using UnityEngine;

namespace Framework
{
	using Utils;
	using Serialization;

	namespace SaveSystem
	{
		//References a value in a save data block of type T
		[Serializable]
		public struct SaveDataValueRef<T>
		{
			#region Public Data
			[SerializeField]
			private Type _saveDataType;
			[SerializeField]
			private string _valueID;
			#endregion

#if UNITY_EDITOR
			[NonSerialized]
			public bool _editorCollapsed;
			[NonSerialized]
			public string _editorValueLabel;
#endif

			public static implicit operator string(SaveDataValueRef<T> property)
			{
				return property.ToString();
			}

			public override string ToString()
			{
				if (IsValid())
				{
					string typeName = SystemUtils.GetTypeName(_saveDataType);
					string valueId = _valueID;

#if UNITY_EDITOR
					if (!string.IsNullOrEmpty(_editorValueLabel))
						valueId = _editorValueLabel;
#endif

					return typeName + "." + valueId;
				}

				return "SaveDataValueRef";
			}

			public bool IsValid()
			{
				return _saveDataType != null;
			}

			public object GetSaveDataValue()
			{
				SaveDataBlock data = SaveData.GetByType(_saveDataType);

				if (data != null)
				{
					return SerializedObjectMemberInfo.GetSerializedFieldInstance(data, _valueID);
				}

				return null;
			}

			public void SetSaveDataValue(object value)
			{
				SaveDataBlock saveData = SaveData.GetByType(_saveDataType);

				if (saveData != null && value != null)
				{
					saveData.SetValue(_valueID, value);
				}
			}

#if UNITY_EDITOR
			public SaveDataValueRef(Type saveDataType)
			{
				_saveDataType = saveDataType;
				_valueID = string.Empty;
				_editorCollapsed = false;
				_editorValueLabel = string.Empty;
			}

			public SaveDataValueRef(Type saveDataType, string valueID, string valueLabel)
			{
				_saveDataType = saveDataType;
				_valueID = valueID;
				_editorCollapsed = false;
				_editorValueLabel = valueLabel;
			}

			public Type GetSaveDataType()
			{
				return _saveDataType;
			}

			public string GetSaveValueID()
			{
				return _valueID;
			}

			public SaveDataBlock CreateEditorSaveBlockInstance()
			{
				if (_saveDataType !=null)
					return Activator.CreateInstance(_saveDataType) as SaveDataBlock;

				return null;
			}

			public object CreateEditorValueInstance()
			{
				SaveDataBlock newData = CreateEditorSaveBlockInstance();

				if (newData != null)
				{
					return SerializedObjectMemberInfo.GetSerializedFieldInstance(newData, _valueID);
				}

				return null;
			}
#endif
		}
	}
}