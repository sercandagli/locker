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
