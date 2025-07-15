using Bright.Serialization;
using System.Collections.Generic;
using SimpleJSON;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

{{ 
    name = x.name
    key_type = x.key_ttype
    key_type1 =  x.key_ttype1
    key_type2 =  x.key_ttype2
    value_type =  x.value_ttype
}}

namespace cfg.blobstruct{
public struct Config{{name}}s
{
    public BlobArray<Config{{name}}> config{{name}}s;

    {{~if x.is_map_table ~}}
    public ref Config{{name}} Get({{cs_define_type key_type}} id)
    {
        for (int i = 0; i < config{{name}}s.Length; i++)
        {
            if (config{{name}}s[i].{{x.index_field.convention_name}} == id)
            {
                return ref config{{name}}s[i];
            }
        }
        Debug.LogError($"Error! Can't find config with key->{{x.index_field.convention_name}}:{id} in {{name}},Check Config Plz!");
        return ref config{{name}}s[0];
    }
    {{~end~}}

}
}