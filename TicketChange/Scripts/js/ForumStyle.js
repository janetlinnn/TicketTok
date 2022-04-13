// 討論版 

// CSS 樣式切換
const filter  = document.querySelector("#main-tabs");

filter.addEventListener("click", changeTab);

function changeTab(e) {
    // console.log(e.target.dataset.status); //檢查點到哪一個
    filterStatus = e.target.dataset.status;
    console.log(filterStatus);
    // 把 filter 的 class="active" 刪除，當點擊到的時候再新增上去
    let allFilter = document.querySelectorAll("#main-tabs li");

    allFilter.forEach((item) => {
        item.classList.remove("focusOn");
    });
    e.target.classList.add("focusOn");
}


