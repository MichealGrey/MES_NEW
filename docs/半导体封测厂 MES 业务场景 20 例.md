# 半导体封测厂 MES 业务场景 20 例

> 本文档包含 20 个半导体封测厂从收到来料到成品仓的完整 MES 业务场景，涵盖不同难度级别（简单/中等/复杂/极其复杂），用于验证 MES 系统是否满足实际复杂生产需求。

---

## 场景分类说明

| 难度级别 | 场景数量 | 说明 |
|---------|---------|------|
| 🟢 简单 | 3 个 | 基础业务流程，单一操作，无异常处理 |
| 🟡 中等 | 7 个 | 多步骤流程，简单异常处理，基础卡控 |
| 🔴 复杂 | 10 个 | 跨模块联动，多重异常处理，复杂追溯 |
| 🔥 极其复杂 | 10 个 | 多维度 Hold、批量操作、系统联动、客户审计、紧急插单、混合工艺路线 |

---

## 🟢 简单场景（3 个）

### 场景 1：标准工单正常流转

**难度：** 🟢 简单  
**业务阶段：** 工单创建 → 批次流转 → 成品入库  
**涉及模块：** 工单管理、批次管理、进站/出站、成品管理

**场景描述：**
```
客户下达标准产品订单，工厂创建工单 WO-2026001（产品：IC-7nm，封装：BGA，数量：1000 颗）。
系统按标准工艺路线 ASM-BGA-7nm-v2.0（共 12 道工序）创建批次 LOT-2026001。
批次按工序顺序流转：
  DieAttach → WireBond → Mold → Cure → TrimForm → Marking → 
  LaserMark → VisualInspection → CP Test → FT Test → BurnIn → Packing

所有工序良率正常（>98%），无 Hold、无重工、无拆批。
最终产出 950 颗合格品（总良率 95%），按 Grade A（900 颗）、Grade B（50 颗）拆分。
Grade A 成品入库，Grade B 降级入库。
```

**关键业务点：**
- 工单创建时绑定标准 Route
- 批次按 Route 自动流转到下一工序
- 每道工序记录进出站时间、设备、载具、操作员
- 出站时自动计算良率
- Test 完成后自动按 Bin 结果拆分等级
- 成品入库生成入库单

**MES 卡控需求：**
1. 工单下达前检查主数据完整性（产品、Route、设备、载具）
2. 进站前校验批次状态（必须为 Waiting/Released）
3. 进站前校验工序顺序（不能跳过工序）
4. 出站时数量平衡校验（投入=合格 + 不良 + 报废）
5. 成品入库前检查是否完成所有工序

**验收标准：**
```text
✅ 工单创建成功，批次自动初始化
✅ 批次按 Route 工序顺序流转，无跳站
✅ 每道工序记录完整（时间、设备、载具、人）
✅ 良率计算准确
✅ Grade Split 正确
✅ 成品入库单生成
✅ 完整追溯链可查询
```

---

### 场景 2：来料晶圆登记与批次创建

**难度：** 🟢 简单  
**业务阶段：** 来料管理 → 批次初始化  
**涉及模块：** 来料管理、IQC、Wafer Map、批次管理

**场景描述：**
```
仓库收到 5 片晶圆来料（Wafer Lot: WF-20260526-001），来自晶圆厂 Fab-A，产品 IC-7nm。
IQC 检验员进行来料检验，结果 Pass。
工程师导入 Wafer Map（JSON 格式），系统自动计算每片晶圆的 Die 数量：
  Wafer #1: 5000 Die
  Wafer #2: 4980 Die
  Wafer #3: 5010 Die
  Wafer #4: 4995 Die
  Wafer #5: 5005 Die

根据工单 WO-2026001，将每片晶圆拆分为一个 Mother Lot：
  WF-20260526-001 → LOT-2026001 (Wafer #1, 5000 Die)
  WF-20260526-001 → LOT-2026002 (Wafer #2, 4980 Die)
  WF-20260526-001 → LOT-2026003 (Wafer #3, 5010 Die)
  WF-20260526-001 → LOT-2026004 (Wafer #4, 4995 Die)
  WF-20260526-001 → LOT-2026005 (Wafer #5, 5005 Die)

每个 Mother Lot 绑定 Route: ASM-BGA-7nm-v2.0，状态变为 Released to Production。
```

**关键业务点：**
- Wafer Lot 编号规则：WF-YYYYMMDD-NNN
- IQC 检验结果判定（Pass/Fail/Conditional）
- Wafer Map 导入与 Die 数量自动计算
- Wafer Lot 按片拆分为 Mother Lot
- Mother Lot 绑定 Wafer ID 和原始 Die 数量
- 建立追溯链：MotherLot.WaferLotId = WF-20260526-001

**MES 卡控需求：**
1. Wafer Lot 未通过 IQC 检验前禁止拆分
2. Wafer Map 必须导入后才能创建 Mother Lot
3. Mother Lot 的原始数量必须等于 Wafer Map 的 Die 数量
4. Mother Lot 必须绑定 WaferLotId 才能进站

**验收标准：**
```text
✅ Wafer Lot 编号生成符合规则
✅ IQC 检验记录完整
✅ Wafer Map 导入成功，Die 数量计算准确
✅ Mother Lot 创建成功，数量正确
✅ WaferLotId 绑定正确
✅ 追溯链建立成功
```

---

### 场景 3：单批次 Hold 与释放

**难度：** 🟢 简单  
**业务阶段：** 生产执行 → 异常处理  
**涉及模块：** 批次管理、Hold 管理

**场景描述：**
```
LOT-2026001 在 WireBond 工序出站时，良率 94.0%（目标 97.0%）。
系统触发自动 Hold 规则（良率低于阈值），创建 Hold 记录：
  HoldType: YieldHold
  HoldScope: Lot
  HoldReason: "Wire Bond 良率 94.0% 低于目标 97.0%"
  释放条件：工程师确认

工艺工程师赵六收到通知，查看 Hold 详情：
- 不良分布：Wire Bond 拉力不足 8 颗，偏移 4 颗
- 查看设备参数：WB-03 的 Bond Force 参数偏低 0.5N
- 调整设备参数后，要求重工不良品 12 颗

工程师释放 Hold，批次回到 WireBond 工序重新加工。
重工完成后出站，良率 98.5%，继续流转到下一工序。
```

**关键业务点：**
- 自动 Hold 规则触发（良率阈值）
- Hold 记录创建（类型、原因、释放条件）
- Hold 批次禁止进站
- 工程师处理 Hold 根因
- Hold 释放后批次恢复生产
- 重工流程执行

**MES 卡控需求：**
1. Hold 批次状态自动变更为 Hold
2. Hold 批次禁止进站（系统拦截）
3. Hold 释放需要工程师权限
4. Hold/释放记录写入审计日志
5. 重工批次需要记录重工原因和次数

**验收标准：**
```text
✅ 良率低于阈值自动 Hold
✅ Hold 批次禁止进站
✅ Hold 记录完整（类型、原因、释放条件）
✅ Hold 释放后批次恢复生产
✅ 重工流程可追溯
✅ 审计日志完整
```

---

## 🟡 中等场景（7 个）

### 场景 4：工序间拆批与合批

**难度：** 🟡 中等  
**业务阶段：** 生产执行 → 批次拆分/合并  
**涉及模块：** 批次管理、拆批/合批、追溯管理

**场景描述：**
```
LOT-2026001（数量 250 颗）完成 WireBond 工序，准备流转到 Mold。
发现其中 50 颗需要重工（Wire Bond 拉力不足），剩余 200 颗继续生产。

拆批操作：
1. 选择拆批方式：按数量拆分
2. 母批 LOT-2026001 保留 200 颗，继续生产
3. 子批 LOT-2026001-R1 拆分 50 颗，进入重工路线 RW-WB-001
4. 建立追溯链：LOT-2026001-R1.MotherLotId = LOT-2026001
5. 记录拆批原因：Wire Bond 重工
6. 领班电子签名确认

重工路线 RW-WB-001：
  RW-WB-001（重工 WireBond）→ 回到 WireBond → 继续正常路线

LOT-2026001-R1 重工完成后，回到 Mold 工序。
同时 LOT-2026006（同产品、同工序、同状态，数量 30 颗）也在等待。

合批操作：
1. 选择两个批次：LOT-2026001-R1（50 颗）+ LOT-2026006（30 颗）
2. 系统校验：
   ✅ ProductId 相同（IC-7nm）
   ✅ RouteId 相同（ASM-BGA-7nm）
   ✅ CurrentStep 相同（Mold）
   ✅ 状态都是 Waiting
   ✅ 合批后数量不超载具容量（80 <= 500）
3. 合并为新批次 LOT-2026008（80 颗）
4. 保留追溯链：LOT-2026008.SourceLotIds = [LOT-2026001-R1, LOT-2026006]
```

**关键业务点：**
- 拆批条件校验（状态、工序、权限）
- 重工路线自动切换
- 追溯链自动建立（MotherLotId）
- 合批条件校验（产品、Route、工序、状态、容量）
- 追溯链保留（SourceLotIds）

**MES 卡控需求：**
1. Processing 中禁止拆批
2. Hold 状态禁止拆批
3. 不同产品禁止合批
4. 不同 Route 禁止合批
5. 不同工序禁止合批
6. 超载具容量禁止合批
7. 拆批/合批需要电子签名

**验收标准：**
```text
✅ 拆批条件校验通过
✅ 重工路线切换成功
✅ 追溯链建立正确
✅ 合批条件校验通过
✅ 合批后追溯链保留
✅ 电子签名记录完整
```

---

### 场景 5：按 Bin 等级拆分（Grade Split）

**难度：** 🟡 中等  
**业务阶段：** Test 完成 → 成品拆分  
**涉及模块：** 批次管理、测试管理、成品管理

**场景描述：**
```
LOT-2026001 完成所有 Test 工序（CP Test → FT Test → BurnIn），测试数据上传：
  Bin1（Grade A）: 4500 颗
  Bin2（Grade B）: 300 颗
  Bin3（Grade C）: 100 颗
  总投入：5000 颗
  总良率：98.0%

系统自动触发 Grade Split：
  LOT-2026001（母批）→ 状态变为 Completed
  LOT-2026001-GA（Grade A）: 4500 颗，Grade=A, BinResult=Bin1
  LOT-2026001-GB（Grade B）: 300 颗，Grade=B, BinResult=Bin2
  LOT-2026001-GC（Grade C）: 100 颗，Grade=C, BinResult=Bin3

建立追溯链：
  LOT-2026001-GA.MotherLotId = LOT-2026001
  LOT-2026001-GB.MotherLotId = LOT-2026001
  LOT-2026001-GC.MotherLotId = LOT-2026001

Grade A（车规级）发往客户 A（汽车电子）
Grade B（工业级）发往客户 B（工业控制）
Grade C（消费级）发往客户 C（消费电子）
```

**关键业务点：**
- Test 完成后自动触发 Grade Split
- 按 Bin 结果拆分等级
- 等级批次继承母批追溯链
- 不同等级对应不同客户
- 等级批次独立入库

**MES 卡控需求：**
1. 必须完成所有 Test 工序才能 Grade Split
2. Bin 结果必须上传才能 Grade Split
3. Grade Split 后母批状态变为 Completed
4. 等级批次必须记录 Grade 和 BinResult
5. 等级批次追溯链必须完整

**验收标准：**
```text
✅ Test 完成后自动 Grade Split
✅ 等级批次数量正确
✅ Grade/BinResult 记录正确
✅ 追溯链继承正确
✅ 不同等级独立入库
✅ 客户追溯可查询
```

---

### 场景 6：设备 Down 时自动 Hold 与恢复

**难度：** 🟡 中等  
**业务阶段：** 生产执行 → 设备异常处理  
**涉及模块：** 设备管理、Hold 管理、批次管理

**场景描述：**
```
WB-03 设备（Wire Bond 设备）突发故障，设备工程师将状态改为 Down。
系统自动触发 Equipment Hold 规则：

1. 正在 WB-03 上 Processing 的批次自动 Hold：
   - LOT-2026001（Processing, WireBond, WB-03）→ 自动 Hold
   - HoldType: EquipmentHold
   - HoldReason: "设备 WB-03 Down"

2. 等待使用 WB-03 的 Waiting 批次禁止进站：
   - LOT-2026002（Waiting, WireBond）→ 尝试进站 WB-03 → 系统拦截

3. 系统自动计算影响范围：
   - 影响批次：5 个（3 个 Processing, 2 个 Waiting）
   - 影响工单：WO-2026001, WO-2026003
   - 预计影响交期：2 个 Urgent 工单可能延误

设备工程师维修 WB-03，2 小时后修复。
设备状态改回 Idle，系统自动释放 Equipment Hold：
  - LOT-2026001 自动恢复为 Processing
  - 等待批次可以重新进站

生产主管手动调整优先级，将 Urgent 工单批次优先安排。
```

**关键业务点：**
- 设备 Down 时自动 Hold Processing 批次
- 设备 Down 时禁止 Waiting 批次进站
- 影响范围自动计算（批次、工单、交期）
- 设备恢复后自动释放 Hold
- 优先级动态调整

**MES 卡控需求：**
1. 设备状态为 Down 时禁止进站
2. 设备 Down 时 Processing 批次自动 Hold
3. Equipment Hold 需要设备工程师释放
4. 设备恢复后自动检查 Hold 批次
5. 影响交期时自动告警

**验收标准：**
```text
✅ 设备 Down 时自动 Hold Processing 批次
✅ 设备 Down 时禁止 Waiting 批次进站
✅ 影响范围计算准确
✅ 设备恢复后自动释放 Hold
✅ 交期风险告警
✅ 优先级调整生效
```

---

### 场景 7：重工流程与次数限制

**难度：** 🟡 中等  
**业务阶段：** 生产执行 → 重工管理  
**涉及模块：** 批次管理、重工管理、Hold 管理

**场景描述：**
```
LOT-2026001 在 WireBond 工序出站时，发现 20 颗不良（Wire Bond 拉力不足）。
操作员发起重工流程：

重工流程 1：
1. 拆批：LOT-2026001（230 颗）+ LOT-2026001-R1（20 颗）
2. LOT-2026001-R1 进入重工路线 RW-WB-001
3. 重工工序：RW-WB-001 → WireBond（重工）→ Pre-WB Insp（重工检验）
4. 重工完成后回到 Mold 工序
5. ReworkCount = 1

LOT-2026001-R1 在 Mold 工序出站时，又发现 5 颗不良（封装空洞）。
再次发起重工：

重工流程 2：
1. 拆批：LOT-2026001-R1（15 颗）+ LOT-2026001-R2（5 颗）
2. LOT-2026001-R2 进入重工路线 RW-Mold-001
3. ReworkCount = 2（继承母批的 ReworkCount 累加）

重工流程 3（模拟）：
  如果 LOT-2026001-R2 再次发现不良，ReworkCount 累加到 3。
  系统检查：ReworkCount >= 3 → 触发自动 Hold 规则
  HoldType: QualityHold
  HoldReason: "重工次数达到上限（3 次）"
  释放条件：品质工程师确认 + 客户同意（如为车规产品）

最终处置：
  - 重工 3 次仍不良 → MRB（材料审查委员会）判定
  - MRB 判定：报废（Scrap）
  - 创建 ScrapRecord，记录报废原因、数量、判定人
```

**关键业务点：**
- 重工流程发起（拆批 + 重工路线）
- 重工次数累加（ReworkCount）
- 重工次数超限自动 Hold
- MRB 判定流程
- 报废处理

**MES 卡控需求：**
1. 重工次数上限可配置（默认 3 次）
2. 重工次数达到上限自动 Hold
3. Hold 需要品质工程师 + 客户同意（车规）
4. MRB 判定需要电子签名（Level 3）
5. 报废记录完整（原因、数量、判定人）

**验收标准：**
```text
✅ 重工流程可发起
✅ 重工次数累加正确
✅ 重工超限自动 Hold
✅ MRB 判定流程完整
✅ 报废记录完整
✅ 追溯链记录重工历史
```

---

### 场景 8：载具不匹配与容量校验

**难度：** 🟡 中等  
**业务阶段：** 生产执行 → 进站卡控  
**涉及模块：** 载具管理、进站管理

**场景描述：**
```
场景 8.1：载具类型不匹配
LOT-2026001（产品 IC-7nm，封装 BGA）在 WireBond 工序进站。
标准工艺要求载具类型：Strip

操作员扫描载具：TRAY-001（类型：Tray）
系统校验：
  ✅ 批次状态：Waiting ✓
  ✅ 工序顺序：正确 ✓
  ❌ 载具类型：Tray != Strip ✗
  系统拦截，提示："载具类型不匹配，WireBond 工序需要 Strip"

操作员更换载具：STR-20260526-001（类型：Strip）
系统校验通过，允许进站。

场景 8.2：载具容量超限
LOT-2026002（数量 300 颗）和 LOT-2026003（数量 400 颗）在 Mold 工序等待。
两个批次同产品、同工序、同状态，可以合批。

操作员发起合批：
  合批后数量 = 300 + 400 = 700 颗
  载具：Mold-Strip-001（类型：Strip，容量：500）

系统校验：
  ✅ 产品相同 ✓
  ✅ Route 相同 ✓
  ✅ 工序相同 ✓
  ✅ 状态相同 ✓
  ❌ 合批后数量 700 > 载具容量 500 ✗
  系统拦截，提示："合批后数量 700 超过载具容量 500"

操作员更换大容量载具：Mold-Strip-002（容量：1000）
系统校验通过，允许合批。
```

**关键业务点：**
- 载具类型与工序要求匹配
- 载具容量校验（合批时）
- 载具占用状态检查
- 载具更换流程

**MES 卡控需求：**
1. 工序定义标准载具类型
2. 进站时校验载具类型
3. 合批时校验载具容量
4. 载具被占用时禁止使用
5. 载具容量可配置

**验收标准：**
```text
✅ 载具类型不匹配时拦截
✅ 载具容量超限时拦截
✅ 载具占用时拦截
✅ 更换载具后重新校验
✅ 载具容量可配置
```

---

### 场景 9：Recipe 未验证与设备能力不匹配

**难度：** 🟡 中等  
**业务阶段：** 生产执行 → 进站卡控  
**涉及模块：** Recipe 管理、设备管理、进站管理

**场景描述：**
```
场景 9.1：Recipe 未验证
LOT-2026001（产品 IC-7nm，封装 BGA）在 WireBond 工序进站。
设备：WB-03
标准 Recipe：RB-WB-BGA-7nm-v1.2

操作员扫描设备 WB-03，系统自动检查 Recipe：
  ✅ 批次状态：Waiting ✓
  ✅ 工序顺序：正确 ✓
  ✅ 载具类型：Strip ✓
  ❌ Recipe 状态：未验证（Recipe 状态=Pending）✗
  系统拦截，提示："Recipe RB-WB-BGA-7nm-v1.2 未验证，禁止进站"

工艺工程师验证 Recipe：
1. 在 WB-03 上试跑 3 片晶圆
2. 检查参数：Bond Force, Bond Temperature, Ultrasonic Power
3. 参数符合规格，Recipe 状态改为 Validated

操作员重新扫描设备 WB-03：
  ✅ Recipe 状态：已验证 ✓
  系统校验通过，允许进站。

场景 9.2：设备能力不匹配
LOT-2026002（产品 IC-7nm，封装 QFN-48pin）在 WireBond 工序进站。
设备：WB-05（支持封装类型：BGA, SOP, QFP）

操作员扫描设备 WB-05：
  ✅ 批次状态：Waiting ✓
  ✅ 工序顺序：正确 ✓
  ✅ 载具类型：Strip ✓
  ✅ Recipe 状态：已验证 ✓
  ❌ 设备能力：WB-05 不支持 QFN-48pin ✗
  系统拦截，提示："设备 WB-05 不支持封装类型 QFN-48pin"

操作员更换设备：WB-06（支持封装类型：BGA, QFN, SOP, QFP）
系统校验通过，允许进站。
```

**关键业务点：**
- Recipe 验证状态检查
- 设备能力与封装类型匹配
- Recipe 验证流程
- 设备能力可配置

**MES 卡控需求：**
1. Recipe 必须验证后才能使用
2. 设备能力定义支持的封装类型
3. 进站时校验设备能力
4. Recipe 验证需要工艺工程师权限
5. Recipe 版本管理

**验收标准：**
```text
✅ Recipe 未验证时拦截
✅ 设备能力不匹配时拦截
✅ Recipe 验证流程完整
✅ 设备能力可配置
✅ Recipe 版本管理
```

---

### 场景 10：工序停留时间超时告警

**难度：** 🟡 中等  
**业务阶段：** 生产执行 → 超时监控  
**涉及模块：** 批次管理、超时监控、Hold 管理

**场景描述：**
```
LOT-2026002 在 Mold 工序进站时间：2026-05-24 08:00
标准工艺时间：24 小时
当前时间：2026-05-25 10:00（已停留 26 小时）

系统超时监控：
1. 检测到 LOT-2026002 在 Mold 工序停留 26 小时 > 标准 24 小时
2. 创建 Overdue 告警：
   - 批次：LOT-2026002
   - 工序：Mold
   - 超时时长：2 小时
   - 告警级别：Warning

3. 批次列表中超时批次橙色闪烁标识
4. 通知生产主管："LOT-2026002 在 Mold 工序超时 2 小时"

如果超时继续延长（>48 小时）：
1. 告警级别升级为 Critical
2. 触发自动 Hold 规则：
   - HoldType: DataHold
   - HoldReason: "Mold 工序停留时间超过 48 小时"
   - 释放条件：生产主管确认 + 品质工程师确认

生产主管调查原因：
- Mold 设备 MP-02 故障，维修 20 小时
- 维修完成后优先安排 LOT-2026002 出站

品质工程师确认：
- 检查 Mold 工艺参数：温度、压力、时间
- 参数正常，允许释放 Hold
- 批次继续流转到下一工序
```

**关键业务点：**
- 工序停留时间监控
- 超时告警（Warning/Critical）
- 超时自动 Hold
- 超时原因调查
- Hold 释放流程

**MES 卡控需求：**
1. 每道工序定义标准停留时间
2. 超时自动告警（可配置阈值）
3. 严重超时自动 Hold
4. Hold 释放需要权限
5. 超时原因记录

**验收标准：**
```text
✅ 超时自动告警
✅ 超时批次视觉标识
✅ 严重超时自动 Hold
✅ Hold 释放流程完整
✅ 超时原因记录
```

---

### 场景 11：连续不良品超限自动 Hold

**难度：** 🟡 中等  
**业务阶段：** 生产执行 → 质量异常处理  
**涉及模块：** 质量管理、Hold 管理、批次管理

**场景描述：**
```
LOT-2026001 在 WireBond 工序连续生产：
  Die #1: OK
  Die #2: OK
  Die #3: NG（Wire Bond 拉力不足）
  Die #4: NG（Wire Bond 偏移）
  Die #5: NG（Wire Bond 拉力不足）
  Die #6: NG（Wire Bond 短路）
  Die #7: NG（Wire Bond 拉力不足）

系统检测到连续 5 个不良品（连续不良阈值=5）：
1. 触发自动 Hold 规则：
   - HoldType: QualityHold
   - HoldReason: "WireBond 工序连续 5 个不良品"
   - 影响范围：该设备 WB-03 上所有批次

2. 自动 Hold 批次：
   - LOT-2026001（当前批次）→ Hold
   - LOT-2026003（同设备下一批次）→ 禁止进站

3. 通知品质工程师和设备工程师：
   - 品质工程师：调查不良原因
   - 设备工程师：检查设备参数

品质工程师调查：
- 不良模式：Wire Bond 拉力不足（4 个），偏移（1 个）
- 检查设备参数：Bond Force 偏低 0.8N
- 根本原因：设备参数漂移

设备工程师处理：
- 调整 Bond Force 参数
- 重新验证 Recipe
- 试跑 10 颗，全部 OK

品质工程师释放 Hold：
- LOT-2026001 释放，继续生产
- LOT-2026003 允许进站
- 记录 Hold 根因和对策
```

**关键业务点：**
- 连续不良品计数
- 连续不良阈值可配置
- 自动 Hold 当前批次和设备
- 根因调查流程
- Hold 释放流程

**MES 卡控需求：**
1. 连续不良阈值可配置（默认 5 个）
2. 连续不良自动 Hold 当前批次
3. 连续不良自动 Hold 同设备
4. Hold 释放需要品质工程师
5. 根因和对策记录完整

**验收标准：**
```text
✅ 连续不良自动计数
✅ 连续不良超限自动 Hold
✅ 同设备批次自动 Hold
✅ 根因调查流程完整
✅ Hold 释放流程完整
✅ 对策记录完整
```

---

### 场景 12：操作员资质与权限校验

**难度：** 🟡 中等  
**业务阶段：** 生产执行 → 进站卡控  
**涉及模块：** 人员管理、权限管理、进站管理

**场景描述：**
```
场景 12.1：操作员无工序操作资质
新员工李四（EMP-001）在 WireBond 工序进站。
操作员资质：
  - DieAttach: ✓ 有资质
  - WireBond: ✗ 无资质
  - Mold: ✗ 无资质

李四扫描批次 LOT-2026001，准备进站 WireBond：
  ✅ 批次状态：Waiting ✓
  ✅ 工序顺序：正确 ✓
  ✅ 载具类型：Strip ✓
  ✅ 设备状态：Idle ✓
  ❌ 操作员资质：EMP-001 无 WireBond 操作资质 ✗
  系统拦截，提示："操作员李四无 WireBond 工序操作资质"

班组长安排有资质的操作员王五（EMP-002）：
  - WireBond: ✓ 有资质

王五扫描批次 LOT-2026001：
  ✅ 操作员资质：EMP-002 有 WireBond 资质 ✓
  系统校验通过，允许进站。

场景 12.2：操作员无拆批权限
操作员李四（角色：Operator）尝试对 LOT-2026001 拆批：
  ❌ 权限检查：Operator 角色无 Lot.Split 权限 ✗
  系统拦截，提示："操作员李四无拆批权限"

班组长（角色：Leader）执行拆批：
  ✅ 权限检查：Leader 角色有 Lot.Split 权限 ✓
  拆批成功。
```

**关键业务点：**
- 操作员资质定义（工序资质矩阵）
- 进站时校验操作员资质
- 权限矩阵（角色 - 权限映射）
- 关键操作权限校验（拆批、Hold、报废等）

**MES 卡控需求：**
1. 工序定义操作资质要求
2. 操作员资质可配置
3. 进站时校验操作员资质
4. 关键操作校验权限
5. 资质/权限变更记录审计日志

**验收标准：**
```text
✅ 无资质操作员禁止进站
✅ 无权限操作员禁止关键操作
✅ 资质/权限可配置
✅ 资质/权限变更记录审计
```

---

## 🔴 复杂场景（10 个）

### 场景 13：多 Hold 维度并发与优先级处理

**难度：** 🔴 复杂  
**业务阶段：** 生产执行 → 多维度 Hold 处理  
**涉及模块：** Hold 管理、批次管理、权限管理

**场景描述：**
```
2026-05-26 同一天内，LOT-2026001 遭遇多维度 Hold：

08:00 - Lot Hold（单批次级）
  LOT-2026001 在 WireBond 工序良率 94.0%（目标 97.0%）
  系统自动 Hold：
    HoldType: YieldHold
    HoldScope: Lot
    ScopeId: LOT-2026001
    HoldPriority: 9
    释放条件：工艺工程师确认

09:00 - Equipment Hold（设备级）
  WB-03 设备故障 Down
  系统自动 Hold：
    HoldType: EquipmentHold
    HoldScope: Equipment
    ScopeId: WB-03
    HoldPriority: 6
    影响范围：WB-03 上所有 Processing 批次
    释放条件：设备恢复运行

10:00 - Step Hold（工序级）
  WireBond 工序连续 3 个批次良率低于目标
  工艺主管决定 Hold 整个 WireBond 工序：
    HoldType: ProcessHold
    HoldScope: Step
    ScopeId: WireBond
    HoldPriority: 7
    影响范围：所有在 WireBond 工序的批次
    释放条件：工艺验证通过

11:00 - Product Hold（产品级）
  客户投诉 IC-7nm 产品在客户端失效
  品质部决定 Hold 所有 IC-7nm 产品：
    HoldType: CustomerHold
    HoldScope: Product
    ScopeId: IC-7nm
    HoldPriority: 4
    影响范围：所有 IC-7nm 在制批次
    释放条件：品质调查完成 + 客户同意

14:00 - Wafer Lot Hold（来料级）
  发现同 Wafer Lot（WF-20260526-001）的其他 Mother Lot 有异常
  IQC 决定 Hold 整个 Wafer Lot：
    HoldType: MaterialHold
    HoldScope: WaferLot
    ScopeId: WF-20260526-001
    HoldPriority: 3
    影响范围：WF-20260526-001 下所有 Mother Lot
    释放条件：IQC 重新检验

Hold 维度优先级：
  Wafer Lot Hold (3) > Product Hold (4) > Step Hold (7) > Equipment Hold (6) > Lot Hold (9)

释放流程（按优先级从高到低）：
1. 先释放 Wafer Lot Hold（IQC 重新检验 Pass）
2. 再释放 Product Hold（品质调查完成 + 客户同意）
3. 再释放 Step Hold（工艺验证通过）
4. 再释放 Equipment Hold（设备恢复运行）
5. 最后释放 Lot Hold（工艺工程师确认）

只有所有 Hold 都释放后，批次才能继续生产。
```

**关键业务点：**
- 9 种 Hold 维度支持
- Hold 维度优先级判定
- Hold 影响范围自动计算
- Hold 级联释放（按优先级从高到低）
- 多维度 Hold 并发处理

**MES 卡控需求：**
1. Hold 维度优先级可配置
2. 大范围 Hold 自动级联小范围 Hold
3. 释放 Hold 时检查是否有更高优先级 Hold
4. 只有所有 Hold 都释放才能继续生产
5. Hold 影响范围实时计算

**验收标准：**
```text
✅ 9 种 Hold 维度支持
✅ Hold 优先级判定正确
✅ Hold 影响范围计算准确
✅ Hold 级联释放正确
✅ 多维度 Hold 并发处理正确
```

---

### 场景 14：Urgent 工单插单与动态优先级调整

**难度：** 🔴 复杂  
**业务阶段：** 生产计划 → 紧急插单  
**涉及模块：** 工单管理、批次管理、排程管理

**场景描述：**
```
背景：
2026-05-26，工厂正在执行 5 个工单：
  WO-2026001: IC-7nm, BGA, 1000 颗，交期 05-30, 优先级 Normal
  WO-2026002: IC-14nm, QFN, 2000 颗，交期 05-28, 优先级 Normal
  WO-2026003: IC-28nm, SOP, 1500 颗，交期 06-05, 优先级 Normal
  WO-2026004: IC-7nm, BGA, 500 颗，交期 05-29, 优先级 Normal
  WO-2026005: IC-14nm, QFP, 1000 颗，交期 06-01, 优先级 Normal

10:00 - 紧急插单
  重要客户 A（车规）下达紧急订单：
    WO-2026006: IC-7nm, BGA, 2000 颗，交期 05-27（明天）, 优先级 Urgent

  生产计划员评估：
  - 当前 WIP 中，IC-7nm BGA 产品有 3 个工单（WO-2026001/004/006）
  - WireBond 工序是瓶颈（设备 WB-03/WB-06 已满负荷）
  - 必须调整优先级才能保证 WO-2026006 交期

  计划员调整优先级：
  1. WO-2026006: Normal → Urgent（最高优先级）
  2. WO-2026001: Normal → High（次高）
  3. WO-2026002: Normal → Medium
  4. WO-2026003: Normal → Low（可延后）
  5. WO-2026004: Normal → High
  6. WO-2026005: Normal → Medium

  系统动态调整：
  1. WIP 列表重新排序：Urgent 工单批次排最前
  2. 瓶颈工序（WireBond）派工清单调整：
     - 优先安排 WO-2026006 的批次
     - 其次 WO-2026001/004 的批次
     - 最后 WO-2026002/003/005 的批次
  3. 设备调度：
     - WB-03/WB-06 优先处理 Urgent 批次
     - 其他设备正常排程

  交期风险计算：
  - WO-2026006: 风险 High（交期只剩 1 天，WIP 进度 30%）
  - WO-2026001: 风险 Medium（交期 4 天，WIP 进度 60%）
  - WO-2026002: 风险 Low（交期 2 天，WIP 进度 80%）
  - WO-2026003: 风险 Low（交期 10 天，WIP 进度 20%）

  生产主管决策：
  1. 安排夜班加班，优先生产 WO-2026006
  2. 临时调配设备 WB-07（备用设备）支持 WireBond
  3. 通知仓库明天优先出货 WO-2026006

  最终结果：
  - WO-2026006 按时交付（05-27）
  - WO-2026001 延迟 1 天交付（05-31）
  - 其他工单正常交付
```

**关键业务点：**
- 紧急工单插单
- 动态优先级调整
- WIP 列表动态排序
- 瓶颈工序识别
- 交期风险计算
- 设备动态调度

**MES 卡控需求：**
1. 工单优先级可动态调整
2. WIP 列表按优先级排序
3. 瓶颈工序优先派工给 Urgent 工单
4. 交期风险实时计算
5. 设备调度支持优先级

**验收标准：**
```text
✅ 紧急工单插单成功
✅ 优先级动态调整生效
✅ WIP 列表按优先级排序
✅ 瓶颈工序优先派工
✅ 交期风险计算准确
✅ 设备调度支持优先级
```

---

### 场景 15：跨工艺路线切换（Assemble → Test → Rework）

**难度：** 🔴 复杂  
**业务阶段：** 生产执行 → 工艺路线切换  
**涉及模块：** 批次管理、工艺路线管理、重工管理

**场景描述：**
```
背景：
LOT-2026001 执行工艺路线 ASM-BGA-7nm-v2.0（Assemble 前道）：
  DieAttach → WireBond → Mold → Cure → TrimForm → Marking → 
  LaserMark → VisualInspection → (Assemble 完成)

场景 15.1：Assemble → Test 交接
LOT-2026001 完成 Assemble 所有工序，准备进入 Test 阶段。
系统自动触发 Assemble→Test 交接流程：

1. Assemble 完成判定：
   - 所有 Assemble 工序完成 ✓
   - 最终良率 98.5% ✓
   - 批次状态：Assemble Completed

2. Test 阶段拆分（可选）：
   - 根据 Test 需求，LOT-2026001 拆分为 2 个 Sub Lot：
     - LOT-2026001-T1: 2000 颗，进入 CP Test 路线
     - LOT-2026001-T2: 500 颗，进入 FT Test 路线（跳过 CP）

3. 工艺路线切换：
   - LOT-2026001-T1: Route 从 ASM-BGA-7nm-v2.0 → TST-CP-BGA-7nm-v1.0
   - LOT-2026001-T2: Route 从 ASM-BGA-7nm-v2.0 → TST-FT-BGA-7nm-v1.0

4. 追溯链建立：
   - LOT-2026001-T1.MotherLotId = LOT-2026001
   - LOT-2026001-T2.MotherLotId = LOT-2026001
   - LOT-2026001-T1.ProcessStage = Test
   - LOT-2026001-T2.ProcessStage = Test

场景 15.2：Test → Rework 路线切换
LOT-2026001-T1 在 CP Test 工序测试不良（Bin3），需要重工：

1. 拆批：
   - LOT-2026001-T1（合格品）: 1800 颗，继续 FT Test
   - LOT-2026001-T1-R1（不良品）: 200 颗，进入重工路线

2. 重工路线切换：
   - LOT-2026001-T1-R1: Route 从 TST-CP-BGA-7nm-v1.0 → RW-CP-001
   - 重工工序：RW-CP-001 → 分析不良原因 → 重工（如可能）→ 重测 CP

3. 重工完成：
   - 重工后 150 颗合格（Bin1/Bin2），50 颗报废
   - 合格品回到 FT Test 工序
   - Route 从 RW-CP-001 → TST-FT-BGA-7nm-v1.0

4. 追溯链更新：
   - LOT-2026001-T1-R1.MotherLotId = LOT-2026001-T1
   - LOT-2026001-T1-R1.ReworkCount = 1
   - 记录重工原因：CP Test Bin3

最终追溯链：
  WF-20260526-001 (Wafer Lot)
  → LOT-2026001 (Mother Lot, Assemble)
    → LOT-2026001-T1 (Sub Lot, Test CP)
      → LOT-2026001-T1-R1 (Rework Lot)
        → LOT-2026001-T1-R1-GA (Grade A, FT Test Bin1)
        → LOT-2026001-T1-R1-GB (Grade B, FT Test Bin2)
```

**关键业务点：**
- Assemble→Test 工艺路线切换
- Test 阶段批次拆分
- 重工路线切换
- 跨路线追溯链建立
- 路线切换时状态同步

**MES 卡控需求：**
1. Assemble 完成后才能切换到 Test
2. Test 路线必须与 Assemble 路线匹配
3. 重工路线必须定义返回点
4. 路线切换时批次状态同步
5. 追溯链必须完整记录路线切换

**验收标准：**
```text
✅ Assemble→Test 路线切换成功
✅ Test 阶段批次拆分正确
✅ 重工路线切换成功
✅ 跨路线追溯链完整
✅ 路线切换时状态同步
```

---

### 场景 16：Wafer Map 与 Die 级追溯

**难度：** 🔴 复杂  
**业务阶段：** 来料管理 → 生产追溯  
**涉及模块：** Wafer Map、批次管理、追溯管理

**场景描述：**
```
背景：
Wafer Lot: WF-20260526-001（5 片晶圆），产品 IC-7nm。
每片晶圆的 Wafer Map 记录每个 Die 的坐标和状态：

Wafer #1 Wafer Map（部分）：
  Die (0,0): Good
  Die (0,1): Good
  Die (0,2): Bad (缺陷类型：划伤)
  Die (1,0): Good
  Die (1,1): Bad (缺陷类型：颗粒)
  ...
  总 Die 数：5000
  Good Die: 4900
  Bad Die: 100

Mother Lot 创建：
  WF-20260526-001 → LOT-2026001 (Wafer #1, 5000 Die)
  LOT-2026001 绑定 Wafer Map: WM-20260526-001

场景 16.1：Die 级追溯（客户投诉）
客户 A（车规）投诉：
  批次：LOT-2026001-GA（Grade A 成品）
  失效 Die 坐标：X=125, Y=89

MES 追溯流程：
1. 查询 LOT-2026001-GA 的追溯链：
   LOT-2026001-GA ← LOT-2026001-T1 ← LOT-2026001 ← WF-20260526-001

2. 查询 Wafer Map：
   Wafer #1, Die (125, 89): Good（原始状态）

3. 查询 OperationHistory：
   - DieAttach: 设备 DB-01, 2026-05-26 07:30
   - WireBond: 设备 WB-03, 2026-05-26 08:30
   - Mold: 设备 MP-02, 2026-05-26 10:00
   - ...

4. 查询同坐标其他 Die：
   - Die (124, 89): 正常
   - Die (126, 89): 正常
   - Die (125, 88): 正常
   - Die (125, 90): 不良（Wire Bond 拉力不足）

5. 根因分析：
   - 失效集中在 (125, 89) 附近
   - Wire Bond 工序异常（WB-03 设备参数漂移）

6. 扩大 Hold 范围：
   - Hold 同 Wafer Map 的相邻 Die（坐标范围：120-130, 85-95）
   - Hold 同设备 WB-03 的其他批次

场景 16.2：Wafer Map 良率热力图
系统生成 Wafer Map 良率热力图：
  - 绿色区域：良率 >98%
  - 黄色区域：良率 95%-98%
  - 红色区域：良率 <95%

  发现：
  - Wafer 边缘区域良率偏低（红色）
  - Wafer 中心区域良率正常（绿色）

  工艺工程师分析：
  - 边缘区域 Wire Bond 拉力不足
  - 原因：Wafer 边缘厚度不均
  - 对策：调整 Wire Bond 设备参数（边缘区域补偿）
```

**关键业务点：**
- Wafer Map 导入与存储
- Die 级坐标追溯
- Wafer Map 与 Mother Lot 绑定
- Die 级良率分析
- Wafer Map 热力图

**MES 卡控需求：**
1. Wafer Map 必须导入后才能创建 Mother Lot
2. Mother Lot 必须绑定 Wafer Map ID
3. Die 坐标必须可追溯
4. Wafer Map 良率可分析
5. 客户投诉时可追溯 Die 级信息

**验收标准：**
```text
✅ Wafer Map 导入成功
✅ Mother Lot 绑定 Wafer Map
✅ Die 坐标可追溯
✅ Wafer Map 良率热力图生成
✅ 客户投诉时可追溯 Die 级信息
```

---

### 场景 17：MRB（材料审查委员会）判定流程

**难度：** 🔴 复杂  
**业务阶段：** 生产执行 → 异常处置  
**涉及模块：** 报废管理、MRB 管理、电子签核

**场景描述：**
```
背景：
LOT-2026001-R2（重工 2 次后的批次）在 WireBond 工序再次发现不良。
ReworkCount = 2，再重工 1 次就达到上限（3 次）。

MRB 触发条件：
1. 重工次数达到上限（3 次）
2. 重复不良模式（同一工序重工 2 次以上）
3. 车规产品不良（客户特殊要求）
4. 高价值批次报废（数量>1000 或金额>$10000）

MRB 流程启动：
1. 系统自动创建 MRB 请求：
   - MRB 编号：MRB-20260526-001
   - 批次：LOT-2026001-R2
   - 不良原因：Wire Bond 拉力不足（重工 2 次）
   - 建议处置：报废

2. MRB 成员组成：
   - 主席：品质主管（张三）
   - 成员：工艺工程师（李四）、生产主管（王五）、客户代表（赵六，远程）

3. MRB 会议（2026-05-26 14:00）：
   - 品质主管展示不良数据：
     - 不良率：15%（目标<3%）
     - 不良模式：Wire Bond 拉力不足
     - 重工历史：2 次重工，效果不佳
   - 工艺工程师分析根因：
     - 设备 WB-03 参数漂移
     - 已调整参数，但该批次受损严重
   - 生产主管评估成本：
     - 批次价值：$15000
     - 再重工成本：$5000
     - 成功率：<30%
   - 客户代表意见：
     - 车规产品，质量第一
     - 同意报废

4. MRB 判定：
   - 处置方式：报废（Scrap）
   - 报废原因：重工超限，不良率超标
   - 报废类别：工艺不良
   - 责任部门：生产部

5. 电子签核（Level 3 - 双人复核）：
   - 操作人：品质主管 张三（EMP-001）
   - 审批人：工厂总经理 钱七（EMP-100）
   - 签核时间：2026-05-26 15:00

6. 报废执行：
   - 批次状态：Scrapped
   - 报废数量：200 颗
   - 报废记录写入 ScrapRecord
   - 追溯链更新：LOT-2026001-R2.Scrapped = true

7. 后续处理：
   - 报废批次送废品仓
   - 财务记账：报废损失$15000
   - 品质部发起 CAPA（纠正与预防措施）
```

**关键业务点：**
- MRB 触发条件
- MRB 成员组成
- MRB 会议流程
- MRB 判定与电子签核
- 报废执行与追溯

**MES 卡控需求：**
1. MRB 触发条件可配置
2. MRB 成员可配置（按产品/不良类型）
3. MRB 判定需要电子签核（Level 3）
4. 报废执行后批次状态锁定
5. MRB 记录完整（会议记录、判定、签核）

**验收标准：**
```text
✅ MRB 触发条件正确
✅ MRB 成员组成正确
✅ MRB 会议流程完整
✅ MRB 判定电子签核完整
✅ 报废执行后批次状态锁定
✅ MRB 记录完整可追溯
```

---

### 场景 18：客户特殊要求管控（车规产品）

**难度：** 🔴 复杂  
**业务阶段：** 生产执行 → 客户特殊要求  
**涉及模块：** 客户管理、质量管理、批次管理

**场景描述：**
```
背景：
客户 A（车规电子）下达工单 WO-2026006，产品 IC-7nm（车规级）。
客户特殊要求（Customer Specific Requirements, CSR）：

1. 质量要求：
   - 良率目标：99.0%（标准 97.0%）
   - 重工次数限制：2 次（标准 3 次）
   - 零缺陷交付（0 PPM）

2. 追溯要求：
   - Die 级追溯（必须追溯到 Wafer Map 坐标）
   - 设备参数追溯（每道工序的设备参数）
   - 操作员资质追溯（每道工序的操作员资质）

3. 测试要求：
   - 100% 全检（标准 AQL 抽样）
   - 老化测试 168 小时（标准 48 小时）
   - 温度循环测试 -40°C~125°C（1000 次）

4. 包装要求：
   - 防静电包装（ESD Safe）
   - 湿度指示卡（HIC）
   - 真空包装

5. 文件要求：
   - 出货检验报告（COA）
   - 材质证明（MSDS）
   - 可靠性测试报告（Reliability Report）

MES 管控流程：
1. 工单创建时绑定 CSR：
   - WO-2026006.CustomerId = Customer-A
   - WO-2026006.CSR = {
       YieldTarget: 99.0%,
       MaxReworkCount: 2,
       ZeroDefect: true,
       DieTraceability: true,
       EquipmentParamTrace: true,
       OperatorQualification: true,
       100PercentInspection: true,
       BurnInHours: 168,
       TempCycle: 1000,
       ESDPackage: true,
       COA: true,
       MSDS: true,
       ReliabilityReport: true
     }

2. 批次初始化时继承 CSR：
   - LOT-2026006.CustomerId = Customer-A
   - LOT-2026006.CSR = WO-2026006.CSR
   - LOT-2026006.IsAutomotive = true（车规标识）

3. 生产执行时 CSR 卡控：
   - 进站时校验操作员资质（必须车规资质）
   - 出站时良率阈值 99.0%（标准 97.0%）
   - 重工次数限制 2 次（标准 3 次）
   - 设备参数自动记录（EAP 集成）
   - 100% 全检（跳过 AQL 抽样）

4. 测试阶段 CSR 卡控：
   - BurnIn 时间 168 小时（标准 48 小时）
   - 温度循环 1000 次（标准 0 次）
   - 测试数据 100% 上传

5. 成品阶段 CSR 卡控：
   - 包装类型：ESD Safe + HIC + 真空
   - 文件生成：COA + MSDS + Reliability Report
   - 出货前品质 Gate 检查（QA 放行）

6. 客户追溯报告：
   - 客户 A 可随时查询批次进度
   - 导出完整追溯报告（Die 级）
   - 导出设备参数记录
   - 导出操作员资质记录

7. 出货时 CSR 验证：
   - 检查所有 CSR 要求是否满足
   - 任何一项不满足 → 禁止出货
   - QA 经理电子签名放行
```

**关键业务点：**
- 客户特殊要求定义
- CSR 绑定工单和批次
- CSR 在生产执行时卡控
- CSR 验证与放行
- 客户追溯报告生成

**MES 卡控需求：**
1. CSR 可配置（按客户/产品）
2. CSR 绑定工单和批次
3. 生产执行时 CSR 自动卡控
4. CSR 不满足禁止出货
5. 客户追溯报告可生成

**验收标准：**
```text
✅ CSR 可配置
✅ CSR 绑定工单和批次
✅ CSR 在生产执行时自动卡控
✅ CSR 不满足禁止出货
✅ 客户追溯报告可生成
✅ 车规产品零缺陷交付
```

---

### 场景 19：班次交接与待办事项传递

**难度：** 🔴 复杂  
**业务阶段：** 生产执行 → 班次交接  
**涉及模块：** 班次管理、待办事项、生产监控

**场景描述：**
```
背景：
工厂实行三班倒：
  - 早班：07:00-15:00
  - 中班：15:00-23:00
  - 夜班：23:00-07:00

场景 19.1：早班→中班交接（2026-05-26 15:00）
早班领班张三准备交班给中班领班李四。

早班生产汇总：
  - 完成批次：12 个
  - 进站：28 个
  - 出站：25 个
  - Hold: 3 个
  - 报废：2 个
  - 平均良率：97.8%
  - 达成率：94.2%

进行中批次（中班继续）：
  - LOT-2026001: IC-7nm, WireBond, WB-03, 250 颗，🔴Urgent, 预计完成:19:00
  - LOT-2026002: IC-7nm, Mold, MP-02, 750 颗，Normal, 预计完成:22:00
  - LOT-2026004: IC-28nm, FinalTest, TH-04, 1000 颗，Normal, 预计完成:20:00

Hold 批次（需要中班跟进）：
  - LOT-2026003: HoldType:YieldHold, 原因:Wire Bond 良率 94%<目标 97%
    负责人：赵六 (工程师), 状态:处理中，预计释放:18:00

设备状态：
  - 运行中：8 台
  - 空闲：3 台
  - Down: 1 台 (WB-05, 预计 18:30 恢复)
  - 维护中：1 台 (MP-01, 预计 20:00 完成)

早班待办事项：
  1. ☐ LOT-2026003 拉力测试完成后释放 Hold
  2. ☐ WB-05 设备恢复后优先安排 LOT-2026007 进站
  3. ☐ 通知仓库明天来料 WF-20260527-001 需要紧急处理
  4. ☐ 中班 16:00 有客户审厂，准备好追溯报告

质量异常记录：
  - ⚠ LOT-2026003 Wire Bond 良率 94.0% (目标 97.0%) - 已 Hold
  - ⚠ DB-01 设备参数偏移，已调整

交接班会议（15:00-15:15）：
  早班领班张三：
    "今天早班整体顺利，达成率 94.2%。有 3 个 Hold 批次需要跟进，
     其中 LOT-2026003 预计 18:00 释放。WB-05 设备 18:30 恢复，
     恢复后优先安排 LOT-2026007（Urgent）。16:00 有客户审厂，
     追溯报告已准备好。"

  中班领班李四：
    "收到。我会优先跟进 Hold 批次和 Urgent 工单。WB-05 恢复后
     立即安排 LOT-2026007 进站。客户审厂我来接待。"

  双方确认交接单，电子签名：
    - 交班人：张三 (EMP-001)
    - 接班人：李四 (EMP-002)
    - 交接时间：2026-05-26 15:15

场景 19.2：中班→夜班交接（2026-05-26 23:00）
中班领班李四准备交班给夜班领班王五。

中班生产汇总：
  - 完成批次：15 个
  - 进站：22 个
  - 出站：20 个
  - Hold: 1 个（早班遗留的 LOT-2026003 已释放）
  - 报废：1 个
  - 平均良率：98.2%
  - 达成率：96.5%

夜班待办事项：
  1. ☐ LOT-2026008 需要在 02:00 前完成 FT Test（明天早上出货）
  2. ☐ 夜班 01:00 进行设备 PM（预防性维护），WB-03/WB-06 停机 1 小时
  3. ☐ 来料 WF-20260527-001 预计 04:00 到达，需要 IQC 检验

夜班生产计划：
  - 计划完成批次：10 个
  - 计划进站：18 个
  - 计划出站：16 个
  - 重点：LOT-2026008（02:00 前完成）

交接确认：
  - 交班人：李四 (EMP-002)
  - 接班人：王五 (EMP-003)
  - 交接时间：2026-05-26 23:15
```

**关键业务点：**
- 班次生产汇总
- 进行中批次清单
- Hold 批次跟进
- 设备状态交接
- 待办事项传递
- 交接班电子签名

**MES 卡控需求：**
1. 班次自动切换（按时间）
2. 班次生产汇总自动生成
3. 待办事项跨班次传递
4. 交接班需要电子签名
5. 交接记录可追溯

**验收标准：**
```text
✅ 班次自动切换
✅ 班次生产汇总自动生成
✅ 待办事项跨班次传递
✅ 交接班电子签名完整
✅ 交接记录可追溯
```

---

## 🔥 极其复杂场景（10 个）

### 场景 20：多工单混合生产与载具冲突解决

**难度：** 🔥 极其复杂  
**业务阶段：** 生产执行 → 载具调度  
**涉及模块：** 批次管理、载具管理、设备管理、排程管理

**场景描述：**
```
背景：
工厂同时执行 10 个工单，涉及 3 种产品、5 种封装类型。
Mold 工序有 4 台设备（MP-01/02/03/04），使用 Strip 载具。

2026-05-26 10:00，Mold 工序发生载具冲突：

在制批次（10 个）：
  - LOT-2026001: IC-7nm, BGA, Strip-Type-A, MP-01, Processing
  - LOT-2026002: IC-7nm, BGA, Strip-Type-A, MP-02, Waiting
  - LOT-2026003: IC-14nm, QFN, Strip-Type-B, MP-03, Processing
  - LOT-2026004: IC-14nm, QFN, Strip-Type-B, MP-04, Waiting
  - LOT-2026005: IC-28nm, SOP, Strip-Type-C, 无可用设备, Waiting
  - LOT-2026006: IC-7nm, BGA, Strip-Type-A, 无可用设备, Waiting
  - LOT-2026007: IC-28nm, SOP, Strip-Type-C, MP-01, Queue
  - LOT-2026008: IC-14nm, QFN, Strip-Type-B, MP-02, Queue
  - LOT-2026009: IC-7nm, BGA, Strip-Type-A, MP-03, Queue
  - LOT-2026010: IC-28nm, SOP, Strip-Type-C, MP-04, Queue

载具库存：
  - Strip-Type-A: 10 个（已用 4 个，可用 6 个）
  - Strip-Type-B: 8 个（已用 4 个，可用 4 个）
  - Strip-Type-C: 6 个（已用 4 个，可用 2 个）

冲突 1：设备能力冲突
LOT-2026005（IC-28nm, SOP）需要 Mold 工序。
Mold 设备能力：
  - MP-01: 支持 BGA, QFN（不支持 SOP）
  - MP-02: 支持 BGA, QFN（不支持 SOP）
  - MP-03: 支持 BGA, QFN, SOP
  - MP-04: 支持 QFN, SOP

MP-03/MP-04 支持 SOP，但：
  - MP-03: Processing (LOT-2026003, 预计 11:00 完成)
  - MP-04: Waiting (LOT-2026004, 预计 10:30 开始)

解决方案：
1. LOT-2026005 排队等待 MP-04（10:30 可用）
2. 调整优先级：LOT-2026005（Urgent）> LOT-2026004（Normal）
3. LOT-2026004 延后到 12:00，LOT-2026005 优先 10:30 进站

冲突 2：载具不足
LOT-2026006（IC-7nm, BGA）需要 Strip-Type-A 载具。
Strip-Type-A 可用 6 个，但：
  - LOT-2026006 需要 2 个载具（数量 500 颗，单载具容量 250）
  - LOT-2026009 需要 1 个载具（数量 200 颗）
  - 总需求：3 个载具，可用 6 个 ✓（足够）

但 LOT-2026001/02/07/09 也在用 Strip-Type-A：
  - LOT-2026001: MP-01, Processing（占用 2 个载具，11:30 释放）
  - LOT-2026002: MP-02, Waiting（占用 2 个载具，10:30 释放）
  - LOT-2026007: MP-01, Queue（占用 2 个载具，12:00 释放）
  - LOT-2026009: MP-03, Queue（占用 1 个载具，11:00 释放）

10:30 时载具释放：
  - LOT-2026002 完成 → 释放 2 个载具
  - 可用载具：6 + 2 = 8 个

解决方案：
1. LOT-2026006 在 10:30 后使用释放的载具
2. 系统预留载具：LOT-2026006 预留 2 个（10:30 后可用）
3. LOT-2026009 使用现有可用载具（1 个）

冲突 3：载具类型错误
操作员准备 LOT-2026005（IC-28nm, SOP）进站 MP-04。
扫描载具：STR-20260526-010（类型：Strip-Type-A）

系统校验：
  ❌ 载具类型：Strip-Type-A != Strip-Type-C（工艺要求）
  系统拦截，提示："载具类型不匹配，SOP 封装需要 Strip-Type-C"

解决方案：
1. 操作员更换载具：STR-20260526-020（类型：Strip-Type-C）
2. 系统校验通过，允许进站

冲突 4：载具容量不足
LOT-2026005（数量 300 颗）和 LOT-2026010（数量 400 颗）在 MP-04 等待。
两个批次同产品、同工序、同状态，可以合批。

操作员发起合批：
  合批后数量 = 300 + 400 = 700 颗
  载具：STR-20260526-020（类型：Strip-Type-C，容量：500）

系统校验：
  ❌ 合批后数量 700 > 载具容量 500
  系统拦截，提示："合批后数量 700 超过载具容量 500"

解决方案：
1. 操作员更换大容量载具：STR-20260526-021（容量：1000）
2. 系统校验通过，允许合批
3. 合并为 LOT-2026011（700 颗）

最终调度结果（10:30 后）：
  - MP-01: LOT-2026001 (Processing, 11:30 完成)
  - MP-02: LOT-2026006 (Waiting, 10:30 开始，优先级 Urgent)
  - MP-03: LOT-2026003 (Processing, 11:00 完成)
  - MP-04: LOT-2026005 (Waiting, 10:30 开始，优先级 Urgent)
  - Queue:
    - LOT-2026002 (10:30 载具释放后，MP-02 可用)
    - LOT-2026004 (12:00 开始，MP-04)
    - LOT-2026007 (12:00 开始，MP-01)
    - LOT-2026008 (11:00 开始，MP-03)
    - LOT-2026009 (11:00 开始，MP-03)
    - LOT-2026010 (12:00 开始，MP-04，与 LOT-2026004 合批)

系统优化建议：
1. 提前 30 分钟预热载具（加热到 Mold 温度）
2. 载具周转率提升 20%
3. 设备利用率提升 15%
```

**关键业务点：**
- 多工单混合生产
- 设备能力匹配
- 载具类型匹配
- 载具容量校验
- 载具调度优化
- 优先级动态调整
- 合批载具容量校验

**MES 卡控需求：**
1. 设备能力定义支持的封装类型
2. 载具类型与封装类型匹配
3. 载具容量可配置
4. 载具占用状态实时跟踪
5. 载具调度支持优先级
6. 合批时校验载具容量

**验收标准：**
```text
✅ 设备能力匹配正确
✅ 载具类型匹配正确
✅ 载具容量校验正确
✅ 载具占用状态实时跟踪
✅ 载具调度支持优先级
✅ 合批载具容量校验正确
✅ 系统优化建议有效
```

---

### 场景 21：多批次追溯链合并与拆分（复杂 Genealogy）

**难度：** 🔥 极其复杂  
**业务阶段：** 生产执行 → 追溯管理  
**涉及模块：** 批次管理、追溯管理、拆批/合批

**场景描述：**
```
背景：
Wafer Lot: WF-20260526-001（5 片晶圆）
  → 创建 5 个 Mother Lot（LOT-2026001~005）
  → 每个 Mother Lot 在 Assemble/Test 交接处拆分
  → Test 完成后按 Grade 拆分
  → 部分批次合批
  → 部分批次重工

完整追溯链：

WF-20260526-001 (Wafer Lot, 5 片晶圆，25000 Die)
│
├── LOT-2026001 (Mother Lot, Wafer #1, 5000 Die, Assemble Route)
│   │ Assemble: DieAttach → WireBond → Mold → ... → VisualInspection
│   │
│   ├── LOT-2026001-T1 (Sub Lot, 4900 Die, Test CP Route)
│   │   │ Test: CP Test → FT Test → BurnIn
│   │   │
│   │   ├── LOT-2026001-T1-GA (Grade A, 4500 Die, Bin1)
│   │   ├── LOT-2026001-T1-GB (Grade B, 300 Die, Bin2)
│   │   └── LOT-2026001-T1-GC (Grade C, 100 Die, Bin3)
│   │
│   └── LOT-2026001-R1 (Rework Lot, 100 Die, RW-WB-001)
│       │ Rework: RW-WB-001 → WireBond → Mold
│       │
│       └── LOT-2026001-R1-GA (Grade A, 80 Die, Bin1)
│           └── 报废：20 Die
│
├── LOT-2026002 (Mother Lot, Wafer #2, 4980 Die)
│   │ Assemble: DieAttach → WireBond → Mold → ...
│   │
│   ├── LOT-2026002-T1 (Sub Lot, 4880 Die, Test CP Route)
│   │   │
│   │   ├── LOT-2026002-T1-GA (Grade A, 4600 Die, Bin1)
│   │   └── LOT-2026002-T1-GB (Grade B, 280 Die, Bin2)
│   │       └── 报废：200 Die
│   │
│   └── LOT-2026002-R1 (Rework Lot, 100 Die)
│       └── 报废：100 Die
│
├── LOT-2026003 (Mother Lot, Wafer #3, 5010 Die)
│   │
│   ├── LOT-2026003-T1 (Sub Lot, 4910 Die, Test CP Route)
│   │   │
│   │   ├── LOT-2026003-T1-GA (Grade A, 4700 Die, Bin1)
│   │   └── LOT-2026003-T1-GB (Grade B, 210 Die, Bin2)
│   │
│   └── 报废：100 Die (Assemble 不良)
│
├── LOT-2026004 (Mother Lot, Wafer #4, 4995 Die)
│   │
│   └── LOT-2026004-T1 (Sub Lot, 4895 Die, Test FT Route，跳过 CP)
│       │
│       ├── LOT-2026004-T1-GA (Grade A, 4600 Die, Bin1)
│       └── LOT-2026004-T1-GB (Grade B, 295 Die, Bin2)
│
└── LOT-2026005 (Mother Lot, Wafer #5, 5005 Die)
    │
    ├── LOT-2026005-T1 (Sub Lot, 4905 Die, Test CP Route)
    │   │
    │   ├── LOT-2026005-T1-GA (Grade A, 4650 Die, Bin1)
    │   └── LOT-2026005-T1-GB (Grade B, 255 Die, Bin2)
    │
    └── 报废：100 Die (Assemble 不良)

合批操作（Test 完成后）：
  LOT-2026001-T1-GA (4500 Die) + LOT-2026002-T1-GA (4600 Die)
  → 合并为 LOT-2026012-GA (9100 Die)
  → SourceLotIds: [LOT-2026001-T1-GA, LOT-2026002-T1-GA]

  LOT-2026003-T1-GA (4700 Die) + LOT-2026004-T1-GA (4600 Die)
  → 合并为 LOT-2026013-GA (9300 Die)
  → SourceLotIds: [LOT-2026003-T1-GA, LOT-2026004-T1-GA]

  LOT-2026005-T1-GA (4650 Die) 独立不合并

最终出货批次：
  - LOT-2026012-GA: 9100 Die（来源：LOT-2026001-T1-GA + LOT-2026002-T1-GA）
  - LOT-2026013-GA: 9300 Die（来源：LOT-2026003-T1-GA + LOT-2026004-T1-GA）
  - LOT-2026005-T1-GA: 4650 Die（来源：LOT-2026005-T1-GA）

客户追溯查询（客户 A 查询 LOT-2026012-GA）：
  客户 A: "我收到的批次 LOT-2026012-GA 是由哪些晶圆生产的？"

  MES 追溯报告：
  LOT-2026012-GA (9100 Die, Grade A)
  ← LOT-2026001-T1-GA (4500 Die)
    ← LOT-2026001-T1 (4900 Die)
      ← LOT-2026001 (5000 Die)
        ← WF-20260526-001 (Wafer #1, 5000 Die)
          ← 晶圆厂：Fab-A
          ← Wafer ID: WAF-001
  ← LOT-2026002-T1-GA (4600 Die)
    ← LOT-2026002-T1 (4880 Die)
      ← LOT-2026002 (4980 Die)
        ← WF-20260526-001 (Wafer #2, 4980 Die)
          ← 晶圆厂：Fab-A
          ← Wafer ID: WAF-002

  完整操作历史：
  - LOT-2026001: 12 道工序的进出站记录
  - LOT-2026002: 12 道工序的进出站记录
  - 合批记录：2026-05-28 10:00, 操作人：张三
  - 测试数据：CP Test/FT Test/BurnIn 完整数据
  - 良率数据：每道工序良率
  - Hold 记录：无
  - 重工记录：无
```

**关键业务点：**
- 多层级追溯链（Wafer→Mother→Sub→Grade）
- 拆批追溯链建立
- 合批追溯链保留
- 重工追溯链
- 跨层级追溯查询

**MES 卡控需求：**
1. 拆批时自动建立追溯链（MotherLotId）
2. 合批时自动保留追溯链（SourceLotIds）
3. 重工时自动记录重工历史（ReworkCount, ReworkReason）
4. 追溯链可跨层级查询
5. 追溯链可图形化展示

**验收标准：**
```text
✅ 多层级追溯链建立正确
✅ 拆批追溯链自动建立
✅ 合批追溯链自动保留
✅ 重工追溯链自动记录
✅ 跨层级追溯查询正确
✅ 追溯链图形化展示
```

---

### 场景 22：低良率自动 Hold 与根因分析（RCA）

**难度：** 🔥 极其复杂  
**业务阶段：** 生产执行 → 质量异常处理  
**涉及模块：** 质量管理、Hold 管理、根因分析

**场景描述：**
```
背景：
2026-05-26，WireBond 工序连续出现低良率：

批次与良率：
  - LOT-2026001: 94.0% (目标 97.0%) → 自动 Hold
  - LOT-2026003: 94.5% (目标 97.0%) → 自动 Hold
  - LOT-2026007: 93.8% (目标 97.0%) → 自动 Hold
  - LOT-2026009: 95.2% (目标 97.0%) → 警告（未 Hold）

系统自动 Hold 规则触发：
1. 单批次良率低于目标 3% → 自动 Hold
2. 同工序连续 3 个批次良率低于目标 → 触发 Step Hold
3. 同设备连续 2 个批次良率低于目标 → 触发 Equipment Hold

自动 Hold 执行：
1. LOT-2026001/003/007 → Lot Hold（单批次级）
   HoldType: YieldHold
   HoldReason: "WireBond 良率低于目标"

2. WireBond 工序 → Step Hold（工序级）
   HoldType: ProcessHold
   HoldScope: Step
   ScopeId: WireBond
   HoldReason: "WireBond 工序连续 3 个批次低良率"
   影响范围：所有在 WireBond 工序的批次

3. WB-03 设备 → Equipment Hold（设备级）
   HoldType: EquipmentHold
   HoldScope: Equipment
   ScopeId: WB-03
   HoldReason: "WB-03 设备连续 2 个批次低良率"
   影响范围：WB-03 上所有批次

根因分析（RCA - Root Cause Analysis）：
品质工程师赵六收到通知，启动根因分析流程：

步骤 1：数据收集
  - 不良模式分布：
    - Wire Bond 拉力不足：60%
    - Wire Bond 偏移：25%
    - Wire Bond 短路：10%
    - 其他：5%
  - 设备参数对比：
    - WB-03: Bond Force 45.2N (标准 46.0N ±0.5N) → 偏低 0.8N
    - WB-06: Bond Force 46.1N (正常)
  - 同设备其他批次：
    - LOT-2026005: 98.5% (正常，WB-06 设备)
    - LOT-2026008: 97.8% (正常，WB-06 设备)
  - 环境参数：
    - 温度：25°C (正常)
    - 湿度：45% (正常)

步骤 2：鱼骨图分析（4M1E）
  - Man（人）：操作员资质正常，无异常
  - Machine（机）：WB-03 设备 Bond Force 偏低 → 可疑
  - Material（料）：金线批次正常，无异常
  - Method（法）：工艺参数正常，无异常
  - Environment（环）：温湿度正常，无异常

  结论：WB-03 设备 Bond Force 参数漂移是根因

步骤 3：对策制定
  - 临时对策：调整 WB-03 设备 Bond Force 参数到 46.0N
  - 永久对策：更换 WB-03 设备 Bond Force 传感器
  - 预防措施：每周校准 Bond Force 参数

步骤 4：对策验证
  - 调整 WB-03 设备参数后，试跑 10 颗：
    - 良率：99.0% (Pass)
  - 释放 Hold 批次：
    - LOT-2026001: 重工不良品后释放
    - LOT-2026003: 重工不良品后释放
    - LOT-2026007: 重工不良品后释放
  - 释放 WireBond 工序 Hold
  - 释放 WB-03 设备 Hold

步骤 5：效果确认
  - 对策后批次良率：
    - LOT-2026010: 98.8% (Pass)
    - LOT-2026011: 99.1% (Pass)
  - 连续 5 个批次良率>98% → 对策有效

步骤 6：标准化
  - 更新设备维护规程：每周校准 Bond Force 参数
  - 更新点检表：增加 Bond Force 参数检查
  - 培训操作员：Bond Force 参数异常识别

RCA 报告：
  - 问题描述：WireBond 工序连续 3 个批次低良率
  - 根因：WB-03 设备 Bond Force 参数漂移
  - 临时对策：调整参数到 46.0N
  - 永久对策：更换 Bond Force 传感器
  - 预防措施：每周校准参数
  - 效果确认：对策后良率恢复到 98%+
  - 标准化：更新维护规程和点检表
```

**关键业务点：**
- 低良率自动 Hold（多维度）
- 根因分析流程（RCA）
- 鱼骨图分析（4M1E）
- 对策制定与验证
- 效果确认与标准化

**MES 卡控需求：**
1. 低良率自动 Hold 规则可配置
2. 多维度 Hold 并发触发
3. RCA 流程可记录（4M1E 分析）
4. 对策验证流程
5. 效果确认与标准化

**验收标准：**
```text
✅ 低良率自动 Hold 触发正确
✅ 多维度 Hold 并发处理正确
✅ RCA 流程完整（4M1E 分析）
✅ 对策制定与验证流程完整
✅ 效果确认与标准化流程完整
```

---

### 场景 23：系统联动（ERP/EAP/WMS/STDF）

**难度：** 🔥 极其复杂  
**业务阶段：** 系统集成 → 外部系统联动  
**涉及模块：** 集成接口、ERP 接口、EAP 接口、WMS 接口、STDF 接口

**场景描述：**
```
背景：
MES 需要与外部系统集成：
  - ERP：工单、物料、财务
  - EAP/SECS-GEM：设备自动化
  - WMS：仓储管理
  - STDF：测试数据格式

场景 23.1：ERP 工单同步
ERP 系统创建销售订单：
  - 订单号：SO-20260526-001
  - 客户：Customer-A（车规电子）
  - 产品：IC-7nm, BGA
  - 数量：10000 颗
  - 交期：2026-06-05

ERP 同步工单到 MES：
  - 工单号：WO-2026006
  - 产品：IC-7nm
  - 封装：BGA
  - 数量：10000 颗
  - 交期：2026-06-05
  - 优先级：Urgent
  - 客户特殊要求：车规级（CSR）

MES 接收工单：
1. 验证工单数据完整性
2. 检查主数据（产品、封装、Route）
3. 创建工单 WO-2026006
4. 绑定客户特殊要求（CSR）
5. 回复 ERP：工单创建成功

场景 23.2：EAP 设备参数自动采集
WB-03 设备（Wire Bond）执行批次 LOT-2026001。
EAP 系统自动采集设备参数：

进站时：
  - EAP 发送 TrackIn 请求到 MES
  - MES 校验批次、设备、Recipe
  - MES 回复：允许进站

生产中：
  - EAP 每 5 秒采集设备参数：
    - Bond Force: 46.0N
    - Bond Temperature: 150°C
    - Ultrasonic Power: 0.8W
    - Bond Time: 0.5s
  - EAP 上传参数到 MES
  - MES 存储到 OperationHistory

出站时：
  - EAP 发送 TrackOut 请求到 MES
  - MES 校验数量、良率
  - MES 回复：允许出站
  - EAP 上传测试结果（STDF 格式）

场景 23.3：WMS 物料齐套检查
工单 WO-2026006 需要生产领料：
  - 晶圆：5 片（Wafer Lot: WF-20260526-001）
  - 金线：10 卷（Material: Gold-Wire-001）
  - EMC: 20kg（Material: EMC-BGA-001）
  - LeadFrame: 100 条（Material: LF-BGA-001）

MES 发起物料齐套检查：
1. MES 发送齐套检查请求到 WMS
2. WMS 检查库存：
   - 晶圆：5 片 ✓
   - 金线：15 卷 ✓
   - EMC: 25kg ✓
   - LeadFrame: 80 条 ✗（缺 20 条）
3. WMS 回复：缺料（LeadFrame 不足）
4. MES 禁止工单下达
5. 采购紧急采购 LeadFrame 20 条
6. WMS 更新库存：LeadFrame 100 条 ✓
7. MES 重新检查：齐套 ✓
8. 工单下达

场景 23.4：STDF 测试数据上传
LOT-2026001 完成 FT Test 工序，测试机台上传 STDF 数据：

STDF 文件：FT-LOT-2026001-20260526.stdf
  - 测试程序：FT-BGA-7nm-v1.0
  - 测试数量：250 颗
  - Bin 结果：
    - Bin1 (Pass): 240 颗
    - Bin2 (Marginal): 8 颗
    - Bin3 (Fail): 2 颗
  - 测试参数：
    - Voltage: 3.3V ±0.1V
    - Current: 10mA ±1mA
    - Frequency: 100MHz ±5MHz

MES 接收 STDF 数据：
1. 解析 STDF 文件
2. 验证数据完整性
3. 存储测试数据到数据库
4. 计算良率：240/250 = 96.0%
5. 判定良率：96.0% < 97.0% → 自动 Hold
6. 创建 Hold 记录
7. 通知工程师处理

场景 23.5：成品入库同步 WMS
LOT-2026001-GA（Grade A 成品）完成包装，准备入库：
  - 批次：LOT-2026001-GA
  - 产品：IC-7nm, BGA
  - 数量：240 颗
  - 等级：Grade A（车规级）
  - 库位：A-01-01（成品仓）

MES 发起入库请求到 WMS：
1. MES 发送入库请求：
   - 批次号：LOT-2026001-GA
   - 产品：IC-7nm, BGA
   - 数量：240
   - 等级：Grade A
   - 库位：A-01-01
2. WMS 创建入库单：IN-20260526-001
3. WMS 分配库位：A-01-01
4. WMS 回复：入库单创建成功
5. MES 更新批次状态：InWarehouse
6. 仓库管理员扫码入库
7. WMS 更新库存：IC-7nm BGA Grade A +240
8. WMS 回复 MES：入库完成
9. MES 更新批次状态：Shipped（MES 生命周期结束）
```

**关键业务点：**
- ERP 工单同步
- EAP 设备参数自动采集
- WMS 物料齐套检查
- STDF 测试数据解析
- 成品入库同步 WMS

**MES 卡控需求：**
1. ERP 接口支持工单同步
2. EAP 接口支持 SECS-GEM 协议
3. WMS 接口支持物料齐套检查
4. STDF 数据解析与存储
5. 成品入库同步 WMS

**验收标准：**
```text
✅ ERP 工单同步成功
✅ EAP 设备参数自动采集
✅ WMS 物料齐套检查正确
✅ STDF 测试数据解析正确
✅ 成品入库同步 WMS 成功
```

---

### 场景 24：车规产品零缺陷交付（Zero Defect）

**难度：** 🔥 极其复杂  
**业务阶段：** 生产执行 → 车规产品特殊要求  
**涉及模块：** 质量管理、客户管理、批次管理

**场景描述：**
```
背景：
客户 A（车规电子）工单 WO-2026006，产品 IC-7nm（车规级）。
车规产品要求：零缺陷交付（0 PPM）

车规特殊要求：
1. 100% 全检（不允许 AQL 抽样）
2. 追溯链完整（Die 级追溯）
3. 设备参数全程记录
4. 操作员资质全程追溯
5. 良率目标 99.0%
6. 重工次数限制 2 次
7. 老化测试 168 小时
8. 温度循环 -40°C~125°C（1000 次）
9. 出货前品质 Gate（QA 放行）
10. 零缺陷交付（0 PPM）

生产执行流程：

步骤 1：来料检验（IQC）
  Wafer Lot: WF-20260526-001（5 片晶圆）
  IQC 检验：
    - 外观检查：✓ Pass
    - 尺寸检查：✓ Pass
    - 电性测试：✓ Pass
  Wafer Map 导入：✓ 完成
  IQC 判定：Pass（车规加严检验）

步骤 2：Assemble 前道
  LOT-2026001~005（5 个 Mother Lot）
  每道工序 100% 全检：
    - DieAttach: 100% AOI 检查 ✓
    - WireBond: 100% AOI + 拉力测试 ✓
    - Mold: 100% X-Ray 检查 ✓
    - TrimForm: 100% 外观检查 ✓
    - Marking: 100% 字符检查 ✓
  良率：99.2%（目标 99.0%）✓

步骤 3：Test 后道
  LOT-2026001-T1~005-T1（5 个 Sub Lot）
  测试流程：
    - CP Test: 100% 全检（25000 颗）
    - FT Test: 100% 全检（24500 颗）
    - BurnIn: 168 小时老化（24000 颗）
    - 温度循环：1000 次（-40°C~125°C）
  测试数据上传：✓ 完成
  Bin 结果：
    - Bin1 (Grade A): 23800 颗（99.17%）
    - Bin2 (Grade B): 150 颗（0.62%）
    - Bin3 (Grade C): 50 颗（0.21%）
  良率：99.17%（目标 99.0%）✓

步骤 4：Grade Split
  按 Bin 结果拆分：
    - LOT-2026001-T1-GA~005-T1-GA（Grade A）: 23800 颗
    - LOT-2026001-T1-GB~005-T1-GB（Grade B）: 150 颗（降级，不交付车规客户）
    - LOT-2026001-T1-GC~005-T1-GC（Grade C）: 50 颗（报废）

步骤 5：成品检验（OQC）
  Grade A 成品 23800 颗，OQC 检验：
    - 外观检查：100% 全检 ✓
    - 电性测试：100% 全检 ✓
    - 包装检查：ESD Safe + HIC + 真空 ✓
    - 文件检查：COA + MSDS + Reliability Report ✓
  OQC 判定：Pass

步骤 6：品质 Gate（QA 放行）
  QA 经理审核：
    - 来料检验：✓ Pass
    - 生产过程：✓ 无异常
    - 测试数据：✓ 完整
    - 追溯链：✓ 完整（Die 级）
    - 设备参数：✓ 完整记录
    - 操作员资质：✓ 完整追溯
    - 良率：99.17% > 99.0% ✓
    - 重工次数：0 次