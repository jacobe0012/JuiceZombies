import os
import sys



# unity工程目录，当前脚本放在unity工程根目录中
project_path = r'D:\JiYu\config'

type=sys.argv[1] 


#svn update %s --accept=theirs 
# 调用unity中我们封装的静态函数
def handle(type):
    #os.system('C:\Program Files\TortoiseSVN\bin svn update %s'%(project_path))
    #os.system( f"svn revert -R {project_path} --username suxiao --password a12345")
    os.system( f"svn update {project_path} --username suxiao --password a123456")


    if type == "Server":
        print("Server")
    else:
        print("Client")
            
            
            
            
            
            
            
            
            
            
            
handle(type)
