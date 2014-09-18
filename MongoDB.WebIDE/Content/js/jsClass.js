//js简单类,主要功能有获取表单的值.

var js=window.js={
    verson:'1.0',
    author:'zhouguoxuan',
    create:function(){alert("create")},
    id:function(e_id){ return document.getElementById(e_id)},
    name:function(e_id){ return document.getElementsByName(e_id)},
    value:function(e_id){ return js.id(e_id).value },
    select:function(e_id){ return  js.id(e_id).options[js.id(e_id).selectedIndex].value;},
    select_text:function(e_id){ return  js.id(e_id).options[js.id(e_id).selectedIndex].text;},
    radio:function(e_id)
    {
        var vvv=js.name(e_id);
        if(vvv.length)
        {
              for(var i=0;i<vvv.length;i++)
              {
               if(vvv[i].checked){ return vvv[i].value;}
              }
        }
    },
    check:function(e_id)
    {
        var vvv=js.name(e_id);
        var values="";
        if(vvv.length)
        {
              for(var i=0;i<vvv.length;i++)
              {
                   if(vvv[i].checked)
                   {
                    values+=vvv[i].value+",";
                   }
              }  
        }
        return values;
    },
    setcookie:function setcookie(name, value, hours) //保存cookie
    {
        var expire = "";
		if(hours != null)
		{
		expire = new Date((new Date()).getTime() + hours * 3600000);//日期转换.把小时转换为某一天
		expire = "; expires=" + expire.toGMTString();
		}
		document.cookie = name  + "=" + escape(value) + expire;
    },
    getcookie:function(name)    //获取cookie
    {
   		var cookieValue = "";
		var search = name + "=";
		if(document.cookie.length > 0)
		{ 
		offset = document.cookie.indexOf(search);
		if (offset != -1)
		{ 
		  offset += search.length;
		  end = document.cookie.indexOf(";", offset);
		  if (end == -1) end = document.cookie.length;
		  cookieValue = unescape(document.cookie.substring(offset, end))
		}
		}
		return cookieValue; 
    
    },
    delcoookie:function()
    {
    
    }
}