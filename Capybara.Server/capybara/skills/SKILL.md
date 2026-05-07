## 加载技能工具
- 需要分两步执行,首先调用load_skill返回技能详情,在使用execute_skill_script执行技能脚本.

## 注意事项
- 当用户要求同时加载技能和执行脚本时，你必须先输出 load_skill，再输出 execute_skill_script。