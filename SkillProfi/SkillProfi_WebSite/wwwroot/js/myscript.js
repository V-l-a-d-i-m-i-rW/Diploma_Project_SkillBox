//проверка строки на пустоту
function isEmpty(str) 
{
	return (str.length === 0 || !str.trim());
	//return (!str || 0 === str.length);
}
//диактивация кнопки
function activateButton(arrTxt, button) 
{
	 var dis = true;
	 for (var i = 0; i < arrTxt.length; i++)
	 {
		dis = dis && !isEmpty(arrTxt[i])
	 }

	  button.disabled = !dis;				
}
//проверка валидности адреса электронной почты
function ValidMail(email) 
{
    var re = /^[A-Z0-9._%+-]+@[A-Z0-9-]+.+.[A-Z]{2,4}$/i;///^[\w-\.]+@[\w-]+\.[a-z]{2,4}$/i;
    return re.test(email);
}
 
/*function ValidPhone() {
    var re = /^\d[\d\(\)\ -]{4,14}\d$/;
    var myPhone = document.getElementById('phone').value;
    var valid = re.test(myPhone);
    if (valid) output = 'Номер телефона введен правильно!';
    else output = 'Номер телефона введен неправильно!';
    document.getElementById('message').innerHTML = document.getElementById('message').innerHTML+'<br />'+output;
    return valid;
}  */
