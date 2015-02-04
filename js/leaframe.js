/*!
 * Leaframe 0.5 (http://leaframe.lignusdev.com)
 * Copyright 2014 Curtis Pelissier
 * Licensed under MIT (https://github.com/DarkRewar/leaframe/blob/master/~/doc/licence)
 */
if (typeof jQuery !== 'undefined') {
    $.fn.extend({
        toggled: function(sens) {
            if (sens == "width") $(this).animate({
                width: "toggle"
            }, 400);
            else $(this).animate({
                height: "toggle"
            }, 400);
        },
        overflow: function(type) {
            if ($(this).css('overflow') != 'hidden') {
                $(this).css({
                    'overflow': 'hidden'
                });
            } else {
                $(this).css({
                    'overflow': 'auto'
                });
            }
        },
        fadeTop: function(e) {
            if ($(this).is(':visible')) {
                $('.out-modal').fadeOut();
                $(this).animate({
                    'top': '-250px'
                }).fadeOut();
            } else {
                $('.out-modal').fadeIn();
                $(this).animate({
                    'top': '50px'
                }).show();
            }
        },
        drop: function(e) {
            var id = '#' + $(this).attr('drop');
            if ($(id).is(':visible')) {
                $(id).hide();
            } else {
                $(id).show();
                if ($(this).parents('.bottom').length > 0) {
                    var h = $(id).height();
                    $(id).css({
                        'margin-top': '-' + h + 'px'
                    });
                } else if ($(this).parents('.right').length > 0) {
                    var w = $(id).width();
                    $(id).css({
                        'margin-left': '-' + w + 'px'
                    });
                } else if ($(this).parents('.left').length > 0) {
                    var w = $(id).width();
                    $(id).css({
                        'margin-left': w + 'px'
                    });
                }
            }
        },
        modal: function() {
            if(event)
                event.preventDefault();
            if ($('.out-modal').length == 0) {
                $('body').append('<div class="out-modal"></div>');
            }
            $(this).stop().fadeTop();
        },
        theaterDraw: function(stat) {
            if (typeof stat == 'undefined') stat = '';
            //$('body').overflow();
            if ($(this).is(':visible') || stat == 'hide') {
                var dis = $(this);
                $('.theater-scene').animate({
                    "top": "-75%"
                }, function() {
                    $(this).hide();
                    dis.hide();
                });
                $('.theater-links').animate({
                    "bottom": "-25%"
                }, function() {
                    $(this).hide();
                    dis.hide();
                });
            } else {
                $(this).show();
                var scene = $(this).find('.theater-scene');
                var links = $(this).find('.theater-links');
                scene.show().animate({
                    "top": "0"
                });
                links.show().animate({
                    'bottom': '0'
                });
            }
        },
        theater: function(e) {
            var id = $(this).parents('[theater]').attr('theater');
            var iid = '#' + id;
            var img = $(this).html();
            if ($(iid).length == 0) {
                $('body').append('<div class="theater" id="' + id + '">' + '<div class="theater-scene" id="' + id + '-scene">' + '<a class="close">&times;</a>' + img + '</div>' + '<div class="theater-links" id="' + id + '-links"></div>' + '</div>');
            } else {
                $(iid).html('<div class="theater-scene" id="' + id + '-scene">' + '<a class="close">&times;</a>' + img + '</div>' + '<div class="theater-links" id="' + id + '-links"></div>');
            }
            $('[theater="' + id + '"]').find('li img').each(function() {
                var limg = $(this).attr('src');
                $(iid + '-links').append($('<li />').attr('to-change', id + '-scene').append($('<img />').attr('src', limg)));
            });
            $(iid).theaterDraw();
        },
        panel: function() {
            var parent = $(this).parents('.tabs');
            var to_show = parent.find($(this));
            if (to_show.is(':visible')) {
                return;
            }
            parent.find('.content.active').fadeOut(350, function() {
                $(this).removeClass('active');
                var id_tab = $(this).attr('id');
                $(parent).find('[show="' + id_tab + '"]').removeClass('active');
                id_tab = $(to_show).attr('id');
                $(parent).find('[show="' + id_tab + '"]').addClass('active');
                to_show.fadeIn(350, function() {
                    $(this).addClass('active');
                });
            });
        },
        affix: function() {
            if ($(this).length > 0) {
                $(this).css('position', 'static');
                var affix = $(this).offset().top;
                var widthBox = $(this).innerWidth();
                var topDecal = ( !! $(this).attr('decal-top')) ? parseInt($(this).attr('decal-top')) : 0;
                var windowTop = $(window).scrollTop() + topDecal;
                if (affix < windowTop) {
                    $(this).css({
                        position: 'fixed',
                        top: topDecal,
                        'width': '100%'
                    });
                }
            }
        },
        accordeon: function() {
            var parent = $(this).parent('.accordeon');
            var sonActive = parent.find('.active');
            var sonHide = sonActive.find('.ac-body');
            sonHide.animate({
                height: "toggle"
            }, 400, function() {
                sonActive.removeClass('active');
            });
            if (!$(this).hasClass('active')) {
                var selfActive = $(this).find('.ac-body');
                var self = $(this);
                selfActive.animate({
                    height: "toggle"
                }, 400, function() {
                    self.addClass('active');
                });
            }
        },
        dropdown: function(){
            var dd = $(this).attr('id').replace('#', ''),
                button = $('[data-drop="'+dd+'"]'),
                left = button.position().left,
                top = button.position().top + button.innerHeight() + 2;
            if($(this).is(':visible')){
                $(this).hide();
            }else{
                $('.dropdown-content').hide();
                $(this)
                    .show()
                    .css({
                        'left': left,
                        'top': top
                    });
            }
        }
    });
    $(document).ready(function() {
        $('[drop]').hover(function() {
            $(this).drop();
        }, function() {
            $(this).drop();
        });
        $('.message .close').click(function() {
            $(this).parent('.message').fadeOut(500, function() {
                $(this).remove();
            });
        });
        $('[modal]').click(function() {
            var id = '#' + $(this).attr('modal');
            $(id).modal();
        });
        $('body').on('click', function(e){
            $('.dropdown-content').hide();
        }).on('click', '.modal .close', function() {
            $(this).parent('.modal').modal();
        }).on('click', '.out-modal', function() {
            $('.modal').modal();
        }).on('click', '[theater] li', function() {
            $(this).theater();
        }).on('click', '.theater .close', function() {
            $(this).parents('.theater').theaterDraw();
        }).on('keydown', function(e) {
            if (e.which == 27) {
                $('.theater').theaterDraw('hide');
            }
        }).on('click', '[to-change]', function() {
            var url = $(this).find('img').attr('src');
            var id = $(this).attr('to-change');
            $('#' + id).find('img').attr('src', url);
        }).on('click', '.tabs>.tab-panel>ul>li', function(e) {
            var id = $(this).attr('show');
            e.preventDefault();
            $('#' + id).panel();
        }).on('click', '.accordeon>.ac-section>.ac-head', function(e) {
            var parent = $(this).parent('.ac-section');
            e.preventDefault();
            $(parent).accordeon();
        }).on('click', '.dropdown', function(e){
            e.stopPropagation();
            var drop = $(this).attr("data-drop");
            $('#'+drop).dropdown();
        }).on('click', '.dropdown-content', function(e){
            e.stopPropagation();
        });
        $(window).scroll(function() { // scroll event
            $('.affix').affix();
        });
    });
}