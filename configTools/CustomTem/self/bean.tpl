using Bright.Serialization;
using System.Collections.Generic;
using SimpleJSON;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

{{
    name = x.name
    parent_def_type = x.parent_def_type
    parent = x.parent
    export_fields = x.export_fields
    hierarchy_export_fields = x.hierarchy_export_fields
}}

namespace cfg.blobstruct{

{{~if x.comment != '' ~}}
/// <summary>
/// {{x.escape_comment}}
/// </summary>
{{~end~}}
public  struct ConfigTb{{name}}
{
    {{~ for field in export_fields ~}}
{{   
    list = field.escape_comment | string.split '_'
}}

    
{{~if field.comment != '' ~}}
    /// <summary>
    /// {{field.escape_comment}}
    /// </summary>
{{~end~}}
    public {{cs_define_type field.ctype}} {{field.convention_name}};
    {{~end~}} 


    //----------------------Read------------------------
    {{~ for field in export_fields ~}}
    {{   
    list = field.escape_comment | string.split '_'
}}
    {{~if field.escape_comment[field.escape_comment.size -1] == '0' ~}}
    if (id == {{list[0]}})
        return chaStats.chaProperty.{{field.convention_name}};
    {{~else}}
    if (id == {{list[0]}})
        return playerData.playerData.{{field.convention_name}};
    {{~end~}}
    {{~end~}} 


    //----------------------Write------------------------
    {{~ for field in export_fields ~}}
    {{   
    list = field.escape_comment | string.split '_'
}}
    {{~if field.escape_comment[field.escape_comment.size -1] == '0' ~}}
    if (id == {{list[0]}})
    {
        chaStats.chaProperty.{{field.convention_name}} += value;
    }
    {{~else}}
    if (id == {{list[0]}})
    {
        playerData.playerData.{{field.convention_name}} += value;
    } 
    {{~end~}}

    {{~end~}} 
        //----------------------参数面板------------------------
        {{num = -1}}  
        {{~ for field in export_fields ~}}
        {{   
        list = field.escape_comment | string.split '_'
        num = num+1 
    }}
        {{~if field.escape_comment[field.escape_comment.size -1] == '0' ~}}
        parasItemsList0[{{num}}].SetInputTxt(chaStats.chaProperty.{{field.convention_name}}.ToString());
        {{~else}}
        parasItemsList0[{{num}}].SetInputTxt(playerData.playerData.{{field.convention_name}}.ToString());
        {{~end~}}
        {{~end~}} 
}
}