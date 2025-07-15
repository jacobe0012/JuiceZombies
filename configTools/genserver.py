from openpyxl import Workbook, load_workbook
from openpyxl.utils import get_column_letter
import exceltool
import template
import genbase
import os
import shutil
import re
import sys
import time
def get_excel_files(folder_path):
    excel_files = []
    for root, dirs, files in os.walk(folder_path):
        for filename in files:
            file_path = os.path.join(root, filename)
            if filename.lower().endswith(".xlsx") or filename.lower().endswith(".xls"):
                excel_files.append(file_path)
    return excel_files

  
    
script_path = os.path.abspath(__file__)
dir_path = os.path.dirname(script_path)
parent_dir = os.path.dirname(dir_path)
#打包工具需要
parent_dir = os.path.dirname(parent_dir)
dir_path = os.path.dirname(parent_dir)
parent_dir = os.path.dirname(parent_dir)
parent_dir = os.path.dirname(parent_dir)

lubanOutputPath = dir_path + r"\Luban\ConfigRoot\Datas"
my_excel = []
folder_paths = []

configDir =parent_dir + r"\config"  
folder_paths.append(configDir)
# /// <summary>
# /// 在这里加入需要转换未处理的excel文件，包含文件后缀
# /// </summary>
my_excel.append(parent_dir+r"\configTools\DiyExcel\attr_self.xlsx")

skilleffectNewName = configDir + r"\skill_effectNew.xlsx"
skilleffectElementName = configDir + r"\skill_effectElement.xlsx"

def gen_base(isClient):

    # 输出的处理过的excel文件夹路径

    genbase.createSkillNew(parent_dir,skilleffectNewName,skilleffectElementName)

    # 获取所有文件夹下的 XLSX 和 XLS 文件路径
    excel_files = []
    for folder_path in folder_paths:
        excel_files.extend(get_excel_files(folder_path))

    my_excel.extend(excel_files)

    tablespath = lubanOutputPath + r"\__tables__.xlsx"
    workbook = load_workbook(tablespath)  # 替换为实际的文件名
    worksheet = workbook['Sheet1']  # 替换为实际的工作表名
    worksheet.delete_rows(2, worksheet.max_row)
    workbook.save(tablespath)
    workbook.close()
    #print('begin')

    # 要移出的元素
    to_remove =configDir + r"\battle\skill_effect.xlsx"

    # 移出特定值的元素
    my_excel.remove(to_remove)

    for index, value in enumerate(my_excel):
        exceltool.genfixedexcel(index, value,lubanOutputPath)


    # 检查文件是否存在
    if os.path.exists(skilleffectNewName):
        # 删除文件
        os.remove(skilleffectNewName)
        os.remove(skilleffectElementName)
        #print(f'文件 {skilleffectNewName} 已成功删除。')
    #else:
        #print(f'文件 {skilleffectNewName} 不存在，无需删除。')
            
    print('done!')
    #isClient=sys.argv[1] 

    #打包工具需要
    #isClient = "True"
    folder_path =dir_path + r"\Luban\Luban.ClientServer\Templates\config\cs_unity_json"
    source_folder_class =dir_path + r"\CustomTem\class"
    source_folder_struct =dir_path + r"\CustomTem\struct"
    source_folder_self =dir_path + r"\CustomTem\self"
    if isClient == True:
        template.handle_template(source_folder_class,folder_path)
        os.system(dir_path + r"\Luban\gen_code_json.bat")
        print("client:已经生成class脚本和json数据")
        template.handle_template(source_folder_struct,folder_path)
        os.system(dir_path + r"\Luban\gen_code_json1.bat")
        print("client:已经生成blob脚本")
    else: 
        template.handle_template(source_folder_self,folder_path)
        os.system(dir_path + r"\Luban\gen_code_json_server.bat")
        print("server:已经生成json数据")
    
gen_base(False)

#input("Press Enter to exit...")









