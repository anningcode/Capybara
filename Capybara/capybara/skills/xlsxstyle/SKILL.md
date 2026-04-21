---
name: xlsxstyle
description: "xlsx样式,创建的所有xlsx文件都需要参考技能包的样式信息,此技能不提供xlsx读写信息,你需要使用其它技能来实现."
---

# 输出要求

## 所有Excel文件

### 人员信息样式
- 加载`references/人员.md`文件,按照描述信息的样式生成Excel文件。
- 用法 load_skill('xlsxstyle','references/人员.md')