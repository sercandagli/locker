(function polyfill() {
  const relList = document.createElement("link").relList;
  if (relList && relList.supports && relList.supports("modulepreload")) {
    return;
  }
  for (const link of document.querySelectorAll('link[rel="modulepreload"]')) {
    processPreload(link);
  }
  new MutationObserver((mutations) => {
    for (const mutation of mutations) {
      if (mutation.type !== "childList") {
        continue;
      }
      for (const node of mutation.addedNodes) {
        if (node.tagName === "LINK" && node.rel === "modulepreload")
          processPreload(node);
      }
    }
  }).observe(document, { childList: true, subtree: true });
  function getFetchOpts(link) {
    const fetchOpts = {};
    if (link.integrity)
      fetchOpts.integrity = link.integrity;
    if (link.referrerPolicy)
      fetchOpts.referrerPolicy = link.referrerPolicy;
    if (link.crossOrigin === "use-credentials")
      fetchOpts.credentials = "include";
    else if (link.crossOrigin === "anonymous")
      fetchOpts.credentials = "omit";
    else
      fetchOpts.credentials = "same-origin";
    return fetchOpts;
  }
  function processPreload(link) {
    if (link.ep)
      return;
    link.ep = true;
    const fetchOpts = getFetchOpts(link);
    fetch(link.href, fetchOpts);
  }
})();
const style = "";
$(".js-form form").on("submit", function(event) {
  let isValid = true;
  const $form = $(this).closest("form");
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  $form.find("[data-validate]").each(function(index, el) {
    const $this = $(el);
    const type = $this[0].type;
    if (type === "checkbox") {
      const $parent = $this.closest(".checkbox-alt");
      if ($this[0].checked) {
        $parent.removeClass("is-error");
        return;
      }
      $parent.addClass("is-error");
    }
    if (type === "text") {
      const $parent = $this.closest(".form__controls");
      console.log($this[0].value);
      if ($this[0].value === "") {
        $parent.addClass("is-error");
        return;
      }
      $parent.removeClass("is-error");
    }
    if (type === "email") {
      const $parent = $this.closest(".form__controls");
      const emailValue = $this[0].value.trim();
      if (!emailRegex.test(emailValue)) {
        $parent.addClass("is-error");
        return;
      }
      $parent.removeClass("is-error");
    }
  });
  isValid = $form.find(".is-error").length === 0 ? true : false;
  if (!isValid) {
    event.preventDefault();
  }
});
$(".js-phone-field").on("input", function(e) {
  const value = e.target.value;
  if (value.length < 4) {
    e.target.value = `+90 `;
  }
  if (value.includes("+90")) {
    return;
  }
  e.target.value = `+90 ${value}`;
});
$(".js-accordion .accordion__head").on("click", function(e) {
  $(this).closest(".accordion__section").toggleClass("is-open").siblings().removeClass("is-open");
  e.preventDefault();
});
const observerCallback = (entries) => {
  entries.forEach((entry) => {
    const isVisible = entry.isIntersecting;
    if (isVisible) {
      entry.target.classList.add("is-visible");
    }
  });
};
const observer = new IntersectionObserver(observerCallback, {
  rootMargin: "-30%"
});
$(".js-skip").on("click", function(event) {
  event.preventDefault();
  const targetData = $(this).data("target");
  const $target = $(`[data-target-element=${targetData}]`);
  $("html, body").animate({ scrollTop: $target.offset().top }, 0);
});
$(window).on("load", () => {
  setTimeout(() => {
    $(".js-animate").each((index, element) => {
      observer.observe(element);
    });
  }, 500);
});
$(window).on("load", () => {
  const $loadingWrapper = $(".js-loading-wrapper");
  $loadingWrapper.addClass("page-loaded");
  setTimeout(() => {
    $loadingWrapper.addClass("hidden");
  }, 300);
});
$(".js-quantity").on("click", ".btn-increase", function(event) {
  event.preventDefault();
  const $countSpan = $(this).closest(".js-quantity").find(".checkbox__count");
  let count = parseInt($countSpan.text());
  count += 1;
  $countSpan.text(count);
});
$(".js-quantity").on("click", ".btn-decrease", function(event) {
  event.preventDefault();
  const $countSpan = $(this).closest(".js-quantity").find(".checkbox__count");
  let count = parseInt($countSpan.text());
  if (count === 0) {
    return;
  }
  count -= 1;
  $countSpan.text(count);
});
const $header = $(".js-header");
const handleNavTriggerClick = (event) => {
  event.preventDefault();
  $header.toggleClass("menu-visible");
};
const handleOutsideClick = (event) => {
  if ($(event.target).closest(".header").length === 0) {
    $header.removeClass("menu-visible");
  }
};
function hideMenuOnDesktop() {
  if ($(this).innerWidth() < 1023) {
    return;
  }
  $header.removeClass("menu-visible");
}
$(window).on("load resize", hideMenuOnDesktop);
$(document).on("click", handleOutsideClick);
$(".js-nav-trigger").on("click", handleNavTriggerClick);
