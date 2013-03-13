
Welcome to SQLMaker
========
SQLMaker is a framework that can output T-SQL through variant replace and statement short circuit.

Getting Started
========
<b>1. Run the \HelloWorld\HelloWorld.exe;</b><br>
<b>2. Copy sql statments below to the left edit box of HelloWorld window;</b><br>
<p>
  {BeginDate=2013-01-01}<br>
  {pid=1,2,3}<br>
  SELECT  *<br>
  FROM 	billdetail b<br>
  WHERE ('{BeginDate}'='' or billdate>='{BeginDate}') <br>
  #ifdef pid<br>
	  and ptypeid in ({pid})<br>
  #endif<br>
</p>
<b>3. Click the button named 'Make';</b><br>
<b>4. If you are lucy to see the transformed sql statements showed in the right edit box, it means SQLMaker is ready for you.</b><br>


Contributing
=======
We encourage you to contribute to SQLMaker! <br>
<b>Join us!</b>



