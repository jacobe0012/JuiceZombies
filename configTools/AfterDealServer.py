import os
import json

script_path = os.path.abspath(__file__)
dir_path = os.path.dirname(script_path)
parent_dir = os.path.dirname(dir_path)
a = parent_dir + r"\configTools\server_json\config_tbactivity.json"
b = parent_dir + r"\configTools\server_json\config_tbdays_challenge.json"
c = parent_dir + r"\configTools\server_json\config_tbdays_sign.json"
d = parent_dir + r"\configTools\server_json\config_tbmonopoly.json"
activity = []
battlepass = []
piggy_bank = []
days_challenge = []
days_sign = []
monopoly = []
# days_sign = []
# battlepass
# piggy_bank
# days_challenge
# days_sign
# monopoly
# turntable

with open(a, 'r', encoding='utf-8') as f:
    activity = json.load(f)

with open(b, 'r', encoding='utf-8') as f:
    days_challenge = json.load(f)

with open(c, 'r', encoding='utf-8') as f:
    days_sign = json.load(f)

with open(d, 'r', encoding='utf-8') as f:
    monopoly = json.load(f)

def find_id_in_array(arr, target_id):
    for index, value in enumerate(arr):
        if value['id'] == target_id:
            return value['tag_func']
    return -1

print(find_id_in_array(days_challenge,1))
for i in range(len(activity)):
    print(activity[i])
    # 修改列表中的每个元素
    if activity[i]['type'] == 11:
        activity[i]['tag_func'] = 3201
    if activity[i]['type'] == 12:
        activity[i]['tag_func'] = 3405
    if activity[i]['type'] == 21:
        activity[i]['tag_func'] = find_id_in_array(days_challenge,activity[i]['link'])
    if activity[i]['type'] == 22:
        activity[i]['tag_func'] = find_id_in_array(days_sign,activity[i]['link'])
    if activity[i]['type'] == 23:
        activity[i]['tag_func'] = find_id_in_array(monopoly,activity[i]['link'])

json_data = json.dumps(activity, indent=2)
with open(a, 'w') as f:
    f.write(json_data)
f.close()