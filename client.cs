function GlassUi::init() {
  new ScriptObject(GlassUi);
  GlassUi.doBackground();
}

function GlassUi::replaceMainMenu() {
  MainMenuButtonsGui.getObject(0).delete();
  mm_Fade.delete();

  MM_Version.delete();
  MM_GlassVersion.delete();
  MM_AuthBar.setVisible(false);

  MM_StartButton.setBitmap("Add-Ons/Ui_BlocklandGlass/ui/btnStartServer");
  MM_JoinButton.setBitmap("Add-Ons/Ui_BlocklandGlass/ui/btnJoinServer");
  MM_PlayerButton.setBitmap("Add-Ons/Ui_BlocklandGlass/ui/btnPlayer");
  MM_OptionsButton.setBitmap("Add-Ons/Ui_BlocklandGlass/ui/btnOption");
  MM_CreditsButton.setBitmap("Add-Ons/Ui_BlocklandGlass/ui/btnCredits");

  MM_QuitButton.setBitmap("Add-Ons/Ui_BlocklandGlass/ui/btnQuit");

  GlassUi::addGradient();
}

function GlassUi::addGradient() {
  %grad = new GuiBitmapCtrl() {
    profile = GuiDefaultProfile;
    bitmap = "Add-Ons/Ui_BlocklandGlass/ui/gradientHorizontal";
    extent = "300" SPC getWord(getRes(), 1);
    mColor = "255 255 255 200";
  };
  MainMenuButtonsGui.add(%grad);
  MainMenuButtonsGui.bringToFront(%grad);
}

function GlassUi::doBackground(%this) {
  %screenshots = 0;
  %file = findFirstFile("screenshots/*");
  while(%file !$= "") {
    if(strPos(%file, ".png") == strLen(%file)-4 || strPos(%file, ".jpg") == strLen(%file)-4 ||strPos(%file, ".jpeg") == strLen(%file)-5) {
      %this.screenshot[%screenshots] = %file;
      %screenshots++;
    }
    %file = findNextFile("screenshots/*");
  }
  %this.screenshots = %screenshots;

  %this.kenBurns1 = new GuiBitmapCtrl(KenBurns) {
    profile = GuiDefaultProfile;
    overflowImage = 1;
    lockAspectRatio = 1;
    position = "0 0";
    extent = getRes();

    mColor = "255 255 255 255";

    bitmap = %this.screenshot[0];
  };

  %this.kenBurns2 = new GuiBitmapCtrl(KenBurns) {
    profile = GuiDefaultProfile;
    overflowImage = 1;
    lockAspectRatio = 1;
    position = "0 0";
    extent = getRes();

    mColor = "255 255 255 0";

    bitmap = %this.screenshot[1];
  };

  MainMenuGui.add(%this.kenBurns1);
  MainMenuGui.add(%this.kenBurns2);
  %this.screenshotId = 1;
}

function GlassUi::nextKenBurns(%this) {
  %this.kenBurnsOdd = !%this.kenBurnsOdd;

  if(%this.kenBurnsOdd) {
    %kb = %this.kenBurns1;
  } else {
    %kb = %this.kenBurns2;
  }

  %id = %this.screenshotId++;
  if(%id >= %this.screenshots)
    %id = %this.screenshotId = 0;

  %kb.setBitmap(%this.screenshot[%id]);
  %kb.randomize();
  %kb.getGroup().pushToBack(%kb);
}

function KenBurns::randomize(%this) {
  %startScale = (getRandom(0, 50)/100) + 1.2; //scale between 1.2 and 1.7
  %endScale = (getRandom(0, 50)/100) + 1.2; //scale between 1.2 and 1.7

  %bufferX = getWord(getRes(), 0) * 0.1;
  %bufferY = getWord(getRes(), 1) * 0.1;

  %offsetStartX = getRandom(-%bufferX, %bufferX);
  %offsetStartY = getRandom(-%bufferY, %bufferY);
  %startX = %offsetStartX-%bufferX;
  %startY = %offsetStartY-%bufferY;

  %offsetEndX = getRandom(-%bufferX, %bufferX);
  %offsetEndY = getRandom(-%bufferY, %bufferY);
  %endX = %offsetEndX-%bufferX;
  %endY = %offsetEndY-%bufferY;

  %this.extent = vectorScale(getRes(), %startScale);
  %this.position = %startX SPC %startY;

  %this.startMotion(%endX SPC %endY, vectorScale(getRes(), %endScale), 200);
}

function KenBurns::startMotion(%this, %endPos, %endExt, %ticks) {
  %this.pos = %this.startPosition = %this.position;
  %this.ext = %this.startExtent = %this.extent;

  %this.endPosition = %endPos;
  %this.endExtent = %endExt;

  %this.ticks = 0;
  %this.tickCap = %ticks;

  %this.tick();
}

function mRound(%num) {
  %dec = %num - mFloor(%num);
  if(%dec < 0.5)
    return mFloor(%num);
  else
    return mCeil(%num);
}

function KenBurns::tick(%this) {
  cancel(%this.kenSch);

  %posDiff = vectorSub(%this.endPosition, %this.startPosition);
  %posDiffX = getWord(%posDiff, 0);
  %posDiffY = getWord(%posDiff, 1);

  %extDiff = vectorSub(%this.endExtent, %this.startExtent);
  %extDiffX = getWord(%extDiff, 0);
  %extDiffY = getWord(%extDiff, 1);

  %this.ticks++;
  if(%this.ticks > %this.tickCap) {
    %this.position = %this.endPosition;
    %this.extent = %this.endExtent;
    %this.mcolor = "255 255 255 0";
    return;
  }

  if(%this.tickCap-40 == %this.ticks) {
    GlassUi.nextKenBurns();
  }

  %opa = %this.ticks/40;
  if(%opa > 1)
    %opa = 1;

  %this.mColor = "255 255 255" SPC (255*%opa);

  //%this.pos = %pos = vectorAdd(%this.pos, %posRate);
  //%this.ext = %ext = vectorAdd(%this.ext, %extRate);

  %float = %this.ticks/%this.tickCap;
  %posX = getWord(%this.startposition, 0);
  %posY = getWord(%this.startposition, 1);
  %extX = getWord(%this.startextent, 0);
  %extY = getWord(%this.startextent, 1);

  %posX = %posX+(%float*%posDiffX);
  %posY = %posY+(%float*%posDiffY);

  %extX = %extX+(%float*%extDiffX);
  %extY = %extY+(%float*%extDiffY);

  %this.position = mRound(%posX) SPC mRound(%posY);
  %this.extent = mRound(%extX) SPC mRound(%extY);

  %this.kenSch = %this.schedule(50, tick);
}

package GlassUi {
  function MainMenuGui::add(%this, %obj) {
    echo("MainMenuGui add: " @ %obj);
    parent::add(%this, %obj);
  }
};
activatePackage(GlassUi);
