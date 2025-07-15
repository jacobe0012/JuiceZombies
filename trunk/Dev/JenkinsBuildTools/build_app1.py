import os
import sys
import time
import requests



# 定义要通知GMURL
GMUrl = r'http://192.168.2.29:9001/pay/wxPay'
FSUrl = r'https://open.feishu.cn/open-apis/bot/v2/hook/e3663f9d-cb88-4a32-b098-4e8f528f6b51'
# 设置你本地的Unity安装目录
unity_exe = r'D:\UnityEditor\6000.0.23f1\Editor\Unity.exe'
# unity工程目录，当前脚本放在unity工程根目录中
project_path = r'D:\JiYu\trunk\Dev'
assets_path =project_path + r'\Assets'
# 日志
log_file = os.getcwd() + r'/unity_log.log'

isTest =True

#buildType=sys.argv[4]
buildType="Full"

cdnURL1 =r"http://192.168.2.224/ApesGang/Android"
# 发送到飞书
def sendFS(isSucc):
    success =""
    if(isSucc):
        success ="成功"
    else:
        success ="失败"

    feishustr={
    "msg_type": "text",
    "content": {
    "text": f"打包结束 \n包名: {productName}\n版本号: {version}\n打包类型: {type}\n打包结果: {success}"}}
    response = requests.post(FSUrl, json = feishustr)
static_func = r'JenkinsBuildTools.Build'
# 杀掉unity进程
def kill_unity():
    os.system(r'taskkill /IM Unity.exe /F')

        
def clear_log():
    if os.path.exists(log_file):
        os.remove(log_file)
#svn update %s --accept=theirs 
# 调用unity中我们封装的静态函数
def call_unity_static_func(func):
    
    #time.sleep(1)
    if isTest:
        #os.system('svn update %s'%(assets_path))
        #os.system('svn revert -R %s'%(assets_path))
        return
    kill_unity()
    clear_log()
    time.sleep(1)
    os.system('svn update %s'%(assets_path))
    os.system('svn revert -R %s'%(assets_path))
    cmd1 = r'start %s -quit -batchmode -nographics -projectPath %s -logFile %s -executeMethod %s --productName:%s --version:%s --type:%s --buildType:%s '%(unity_exe,project_path,log_file,func, productName, version, type ,buildType)
    cmd2 = r'start %s -quit -batchmode -projectPath %s -logFile %s -executeMethod %s --productName:%s --version:%s --type:%s --buildType:%s '%(unity_exe,project_path,log_file,func, productName, version, type ,buildType)
    if type == "Apk":
        os.system(cmd1)
        print('run cmd:  ' + cmd1)
    else:
        os.system(cmd2)
        print('run cmd:  ' + cmd2)

# 实时监测unity的log, 参数target_log是我们要监测的目标log, 如果检测到了, 则跳出while循环    
def monitor_unity_log():
    #打包成功 通知GM后台版本号
    data = {'path': cdnURL1, 'version': version}
    if isTest:
        #发送 POST 请求并传入参数
        sendFS(True)
        requests.post(GMUrl, data = data)

        return
    success_log = 'Build Success!'
    
    failed_log = 'Build Failure!'
    pos = 0
    while True:
        if os.path.exists(log_file):
            break
        else:
            time.sleep(0.1) 
    while True:
        fd = open(log_file, 'r', encoding='utf-8', errors='ignore')
        if 0 != pos:
            fd.seek(pos, 0)
        while True:
            line = fd.readline()
            pos = pos + len(line)
            if success_log in line:
                print(u'Find: --Build App Success:-- ' + success_log)
                fd.close()
                sendFS(True)
                 #打包成功 通知GM后台版本号
                data = {'path': cdnURL1, 'version': version}
                # 发送 POST 请求并传入参数
                requests.post(GMUrl, data = data)
                
                # 检查响应状态和内容
                ##if response.status_code == 200:
                ##    print('GM Request Success：', response.text)
                ##else:
                ##    print('GM Request Fail：', response.text)

                return
            
            if failed_log in line:
                print(u'Find: --Build App Failure:-- ' + failed_log)
                fd.close()
                sendFS(False)
                return
            if line.strip():
                print(line)
            else:
                break
        fd.close()
 
if __name__ == '__main__':
    requests.post(GMUrl, data = "")
    print('done')
