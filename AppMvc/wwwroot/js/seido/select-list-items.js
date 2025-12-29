'use strict';  

function clickHandler (event)  {

    var btn = event.currentTarget;
    var selectedItem = btn.dataset.seidoSelectedItemId;
    
    let elems = document.querySelectorAll('input[data-seido-selected-item-id-target]');
    elems.forEach(elem => {
      elem.value = selectedItem;
    });

    return true;
}

let selems = document.querySelectorAll('*[data-seido-selected-item-id]');
selems.forEach(elem => {
  elem.addEventListener('click', clickHandler);
});
