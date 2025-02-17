# 项目说明
一个角色背包界面的项目，只是个练习的项目Demo。
# 完成目标
- [X] 界面的MVC架构
- [X] 分辨率更好的适配
- [ ] 完成ab框架，进行背包的异步加载
  
# 具体说明
## 关于MVC
- 我选择使用了PureMVC Standard Framework，在此基础上实现了一个基本的UI框架，包括基础的层级管理和UI配置。
- 利用组合模式，在注册中介时把UI预制体和对应中介联系起来，减少UI脚本挂载。
- 不过还是发现一些缺点的，因为PureMVC的通信传参时，是把参数放到object中，存在类型安全问题，且如果参数是值类型并发送频繁，就要考虑装箱和拆箱的开销。考虑解决办法是：
  - 为值类型添加一个泛型参数类，发送消息时把值装入再发送，同时可以用对象池管理该泛型参数类，避免频繁创建/销毁。
  - 或者扩展PureMVC的Notification，使其支持泛型，不过可能需要修改PureMVC核心代码，框架的代码我还没有细看，如果后续完善我会优先用上面的办法。
## 分辨率适配
- 通过挂载脚本自动适配，包括垂直、水平和安全区域适配。