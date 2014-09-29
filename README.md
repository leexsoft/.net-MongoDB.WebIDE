项目结构
========================
----MongoDB.Persist ：读取mongodb信息的持久层
<br/>
----MongoDB.WebIDE  : 主站点程序

使用的技术
========================
<ol>
<li>利用bootstrap(Version 2.3.2)和mvc改写的MongodbManagementStudio</li>
<li>改为使用mongodb .net的官方驱动</li>
<li>使用了ztree控件</li>
<li>使用并行技术读取mongodb信息，解决了填充数据时的线程安全问题</li>
</ol>

功能简介
========================
<ol>
<li>查看服务器，数据库和数据表的统计信息</li>
<li>服务器可查看主从服务器的信息</li>
<li>数据库可查看Profile信息</li>
<li>数据库可查看表数据（暂时仅提供简单查看）</li>
<li>数据库可进行简单的执行计划查看</li>
<li>数据表可进行索引信息查看和新增、删除等管理</li>
</ol>
